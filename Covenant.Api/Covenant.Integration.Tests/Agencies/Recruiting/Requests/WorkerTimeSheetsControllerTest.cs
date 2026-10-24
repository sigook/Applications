using ClosedXML.Excel;
using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Accounting;
using Covenant.Api.Controllers.Agencies.Recruiting.Requests;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Requests.TimeSheets.Agencies;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Requests;

public class WorkerTimeSheetsControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<WorkerTimeSheetsControllerTest.Startup, WorkerTimeSheetsControllerTest.Data>>
{
    private const string Nickname = "recruiter";

    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public WorkerTimeSheetsControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private static string Uri(BookedWorker worker) =>
        WorkerTimeSheetsController.RouteName
            .Replace("{requestId}", worker.Request.Id.ToString())
            .Replace("{workerProfileId}", worker.Profile.Id.ToString());

    private static TimeSheetModel EightHours(DateTime day) => new() { TimeIn = day.AddHours(8), Hours = TimeSpan.FromHours(8) };

    [Fact]
    public async Task Get_ReturnsWorkerTimeSheetsForRequest()
    {
        BookedWorker worker = _data.ForGet;

        var list = await _client.GetFromJsonAsync<List<TimeSheetListModel>>(Uri(worker));

        TimeSheetListModel row = Assert.Single(list);
        Assert.Equal(worker.TimeSheet.Id, row.Id);
        Assert.Equal(worker.TimeSheet.Date, row.Day);
        Assert.True(row.WasApproved);
        Assert.True(row.CanUpdate);
        Assert.Equal(worker.TimeSheet.TimeInApproved, row.TimeInApproved);
        Assert.Equal(worker.TimeSheet.TimeOutApproved, row.TimeOutApproved);
    }

    [Fact]
    public async Task Get_FiltersByDateRange()
    {
        BookedWorker worker = _data.ForGet;
        DateTime day = worker.TimeSheet.Date;

        var list = await _client.GetFromJsonAsync<List<TimeSheetListModel>>(
            $"{Uri(worker)}?startDate={day.AddDays(1):yyyy-MM-dd}&endDate={day.AddDays(5):yyyy-MM-dd}");

        Assert.Empty(list);
    }

    [Fact]
    public async Task Post_CreatesApprovedTimeSheet_WithExtras()
    {
        BookedWorker worker = _data.ForPost;
        TimeSheetModel model = EightHours(Data.FakeNow.AddDays(-2));
        model.BonusOrOthers = 25m;
        model.BonusOrOthersDescription = "Shift bonus";
        model.Reimbursements = 10m;
        model.ReimbursementsDescription = "Parking";
        model.DeductionsOthers = 5m;
        model.DeductionsOthersDescription = "Uniform";
        model.MissingHours = TimeSpan.FromHours(2);
        model.MissingHoursOvertime = TimeSpan.FromHours(1);
        model.MissingRateWorker = 15;
        model.MissingRateAgency = 18;

        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri(worker), model);

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TimeSheetListModel>();
        TimeSheet entity = await Context.TimeSheets.AsNoTracking().SingleAsync(t => t.Id == created.Id);
        Assert.Equal(model.TimeIn.Date, entity.Date);
        Assert.Equal(8, entity.TotalHoursApproved);
        Assert.Equal(25m, entity.BonusOrOthers);
        Assert.Equal(10m, entity.Reimbursements);
        Assert.Equal(5m, entity.DeductionsOthers);
        Assert.Equal(model.MissingHours, entity.MissingHours);
        Assert.Equal(model.MissingHoursOvertime, entity.MissingHoursOvertime);
        Assert.Equal(model.MissingRateWorker, entity.MissingRateWorker);
        Assert.Equal(model.MissingRateAgency, entity.MissingRateAgency);
        Assert.Equal($"Created and approved by {Nickname}", entity.Comment);
    }

    [Fact]
    public async Task Post_SameDateTwice_ReturnsBadRequest()
    {
        BookedWorker worker = _data.ForDuplicate;
        TimeSheetModel model = EightHours(worker.TimeSheet.Date);

        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri(worker), model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(1, await Context.TimeSheets.CountAsync(t => t.WorkerRequestId == worker.WorkerRequest.Id));
    }

    [Fact]
    public async Task Post_WorkerNotBookedOnRequest_ReturnsBadRequest()
    {
        string uri = WorkerTimeSheetsController.RouteName
            .Replace("{requestId}", _data.ForPost.Request.Id.ToString())
            .Replace("{workerProfileId}", Guid.NewGuid().ToString());

        HttpResponseMessage response = await _client.PostAsJsonAsync(uri, EightHours(Data.FakeNow.AddDays(-3)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_DateOlderThanOneYear_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri(_data.ForPost), EightHours(Data.FakeNow.AddYears(-2)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesApprovedHoursAndComment()
    {
        BookedWorker worker = _data.ForPut;
        var model = new TimeSheetModel
        {
            TimeIn = worker.TimeSheet.Date.AddHours(9),
            Hours = TimeSpan.FromHours(6),
            MissingHours = TimeSpan.FromHours(1)
        };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{Uri(worker)}/{worker.TimeSheet.Id}", model);

        response.EnsureSuccessStatusCode();
        TimeSheet entity = await Context.TimeSheets.AsNoTracking().SingleAsync(t => t.Id == worker.TimeSheet.Id);
        Assert.Equal(6, entity.TotalHoursApproved);
        Assert.Equal(model.TimeIn, entity.TimeInApproved);
        Assert.Equal(TimeSpan.FromHours(1), entity.MissingHours);
        Assert.Equal($"Updated and approved by {Nickname}", entity.Comment);
    }

    [Fact]
    public async Task Put_UnknownTimeSheet_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{Uri(_data.ForPut)}/{Guid.NewGuid()}", EightHours(Data.FakeNow));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Usages_UnusedTimeSheet_ReturnsEmptyUsages()
    {
        BookedWorker worker = _data.ForGet;

        var usages = await _client.GetFromJsonAsync<TimeSheetUsagesModel>($"{Uri(worker)}/{worker.TimeSheet.Id}/Usages");

        Assert.Null(usages.InvoiceNumber);
        Assert.Null(usages.PayStubNumber);
    }

    [Fact]
    public async Task Usages_TimeSheetInPayStub_ReturnsPayStubNumber()
    {
        BookedWorker worker = _data.ForPaidUsages;
        string payStubNumber = await GeneratePayStub(worker);

        var usages = await _client.GetFromJsonAsync<TimeSheetUsagesModel>($"{Uri(worker)}/{worker.TimeSheet.Id}/Usages");

        Assert.Equal(payStubNumber, usages.PayStubNumber);
    }

    [Fact]
    public async Task Delete_UnusedTimeSheet_RemovesIt()
    {
        BookedWorker worker = _data.ForDelete;

        HttpResponseMessage response = await _client.DeleteAsync($"{Uri(worker)}/{worker.TimeSheet.Id}");

        response.EnsureSuccessStatusCode();
        Assert.False(await Context.TimeSheets.AnyAsync(t => t.Id == worker.TimeSheet.Id));
    }

    [Fact]
    public async Task Delete_UnknownTimeSheet_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{Uri(_data.ForDelete)}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_TimeSheetInPayStub_ReturnsBadRequest()
    {
        BookedWorker worker = _data.ForPaidDelete;
        await GeneratePayStub(worker);

        HttpResponseMessage response = await _client.DeleteAsync($"{Uri(worker)}/{worker.TimeSheet.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(await Context.TimeSheets.AnyAsync(t => t.Id == worker.TimeSheet.Id));
    }

    [Fact]
    public async Task GetRequestTimeSheetFile_ReturnsExcel()
    {
        BookedWorker worker = _data.ForGet;
        string uri = TimeSheetsController.RouteName.Replace("{requestId}", worker.Request.Id.ToString());

        HttpResponseMessage response = await _client.GetAsync($"{uri}/File");

        response.EnsureSuccessStatusCode();
        using var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        Assert.NotEmpty(workbook.Worksheets);
    }

    private async Task<string> GeneratePayStub(BookedWorker worker)
    {
        (await _client.PostAsJsonAsync($"{PayStubsController.RouteName}/generate", new[] { worker.Profile.WorkerId })).EnsureSuccessStatusCode();
        return await Context.PayStubs.Where(p => p.WorkerProfileId == worker.Profile.Id).Select(p => p.PayStubNumber).SingleAsync();
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => o.AddAdminRole(Data.AgencyId));
            services.AddTestDatabase();
            services.AddSingleton(Rates.DefaultRates);
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton<TimeProvider>(new FakeTimeProvider(new DateTimeOffset(Data.FakeNow, TimeSpan.Zero)));
            services.AddSingleton<AgencyIdFilter>();
            services.AddSingleton(Mock.Of<Covenant.Common.Interfaces.Storage.IPayStubsContainer>());
            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            currentUserService.Setup(s => s.GetAgencyIds()).Returns([Data.AgencyId]);
            currentUserService.Setup(s => s.GetNickname()).Returns(Nickname);
            services.AddSingleton(currentUserService.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}"));
        }
    }

    public class BookedWorker
    {
        public WorkerProfile Profile { get; }
        public Request Request { get; }
        public WorkerRequest WorkerRequest { get; }
        public TimeSheet TimeSheet { get; }

        public BookedWorker(Agency agency, CompanyProfile company, City city, string key)
        {
            Profile = FakeData.FakeWorkerProfile(agency, $"{key}@timesheet-worker.com", city);
            Request = new Request(company, FakeData.FakeJobPositionRate(company)) { AgencyRate = 30, WorkerRate = 20 };
            Request.UpdateJobLocation(FakeData.FakeLocation(city), false);
            WorkerRequest = WorkerRequest.AgencyBook(Profile.Id, Request.Id);
            DateTime day = Data.FakeNow.Date.AddDays(-7);
            TimeSheet = TimeSheet.CreateTimeSheet(WorkerRequest, day, TimeSpan.FromHours(8), now: Data.FakeNow).Value;
            TimeSheet.AddApprovedTime(day.AddHours(8), day.AddHours(16));
        }

        public void Seed(CovenantContext context)
        {
            context.WorkerProfiles.Add(Profile);
            context.Requests.Add(Request);
            context.WorkerRequests.Add(WorkerRequest);
            context.TimeSheets.Add(TimeSheet);
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 18, 10, 00, 00);

        private readonly City city;
        private readonly CompanyProfile company;

        public BookedWorker ForGet { get; }
        public BookedWorker ForPost { get; }
        public BookedWorker ForDuplicate { get; }
        public BookedWorker ForPut { get; }
        public BookedWorker ForDelete { get; }
        public BookedWorker ForPaidUsages { get; }
        public BookedWorker ForPaidDelete { get; }

        private BookedWorker[] Workers => [ForGet, ForPost, ForDuplicate, ForPut, ForDelete, ForPaidUsages, ForPaidDelete];

        public Data()
        {
            city = FakeData.FakeCity();
            Agency agency = FakeData.FakeAgency(AgencyId, city);
            company = FakeData.FakeCompanyProfile(agency, city: city, companyEmail: "timesheets@company.com");
            ForGet = new BookedWorker(agency, company, city, "get");
            ForPost = new BookedWorker(agency, company, city, "post");
            ForDuplicate = new BookedWorker(agency, company, city, "duplicate");
            ForPut = new BookedWorker(agency, company, city, "put");
            ForDelete = new BookedWorker(agency, company, city, "delete");
            ForPaidUsages = new BookedWorker(agency, company, city, "paid-usages");
            ForPaidDelete = new BookedWorker(agency, company, city, "paid-delete");
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(city);
            context.CompanyProfiles.Add(company);
            foreach (BookedWorker worker in Workers) worker.Seed(context);
            context.SaveChanges();
        }
    }
}
