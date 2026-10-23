using ClosedXML.Excel;
using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Accounting;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Requests.TimeSheets.Agencies;
using Covenant.Common.Models;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Accounting;

public class PayStubsControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<PayStubsControllerTest.Startup, PayStubsControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public PayStubsControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private Task<HttpResponseMessage> Generate(params Guid[] workerIds) =>
        _client.PostAsJsonAsync($"{PayStubsController.RouteName}/generate", workerIds);

    private Task<List<WorkerReadyForPayStubModel>> WorkersReady() =>
        _client.GetFromJsonAsync<List<WorkerReadyForPayStubModel>>($"{PayStubsController.RouteName}/WorkersReadyForPayStub");

    [Fact]
    public async Task Generate_CreatesPayStubFromApprovedTimeSheets_AndMarksThemAsPaid()
    {
        PaidWorker worker = _data.ForGenerate;
        Assert.Contains(await WorkersReady(), w => w.WorkerId == worker.WorkerId);

        HttpResponseMessage response = await Generate(worker.WorkerId);

        response.EnsureSuccessStatusCode();
        var payStub = await Context.PayStubs.AsNoTracking()
            .Include(p => p.Items)
            .Include(p => p.WageDetails)
            .SingleAsync(p => p.WorkerProfileId == worker.Profile.Id);
        PayStubItemAssert(payStub.Items.Single(), PayStubItemType.Regular, 16, Data.WorkerRate);
        Assert.Equal(16 * Data.WorkerRate, payStub.GrossPayment);
        Assert.Equal((payStub.GrossPayment * Rates.DefaultRates.Vacations).DefaultMoneyRound(), payStub.Vacations);
        Assert.Equal(payStub.TotalEarnings - payStub.TotalDeductions, payStub.TotalPaid);
        Assert.Equal(2, payStub.WageDetails.Count);
        Assert.StartsWith("PS-", payStub.PayStubNumber);
        Assert.Equal(2, await Context.TimeSheetTotalPayrolls.CountAsync(t => worker.TimeSheetIds.Contains(t.TimeSheetId)));
        Assert.DoesNotContain(await WorkersReady(), w => w.WorkerId == worker.WorkerId);
    }

    [Fact]
    public async Task Generate_Twice_DoesNotDuplicatePayStub()
    {
        PaidWorker worker = _data.ForGenerateTwice;

        (await Generate(worker.WorkerId)).EnsureSuccessStatusCode();
        (await Generate(worker.WorkerId)).EnsureSuccessStatusCode();

        Assert.Equal(1, await Context.PayStubs.CountAsync(p => p.WorkerProfileId == worker.Profile.Id));
    }

    [Fact]
    public async Task Generate_WorkerWithoutApprovedTimeSheets_CreatesNothing()
    {
        HttpResponseMessage response = await Generate(_data.WithoutApprovedTime.WorkerId);

        response.EnsureSuccessStatusCode();
        Assert.False(await Context.PayStubs.AnyAsync(p => p.WorkerProfileId == _data.WithoutApprovedTime.Profile.Id));
        Assert.DoesNotContain(await WorkersReady(), w => w.WorkerId == _data.WithoutApprovedTime.WorkerId);
    }

    [Fact]
    public async Task GetPayStubs_ListsGeneratedPayStub_AsNotDeletable()
    {
        PaidWorker worker = _data.ForList;
        (await Generate(worker.WorkerId)).EnsureSuccessStatusCode();
        var payStub = await Context.PayStubs.AsNoTracking().SingleAsync(p => p.WorkerProfileId == worker.Profile.Id);

        var list = await _client.GetFromJsonAsync<PaginatedList<PayStubListModel>>(
            $"{PayStubsController.RouteName}?payStubNumber={payStub.PayStubNumber}");

        PayStubListModel row = Assert.Single(list.Items);
        Assert.Equal(payStub.Id, row.Id);
        Assert.Equal(payStub.TotalPaid, row.TotalPaid);
        Assert.False(row.CanDelete);
    }

    [Fact]
    public async Task GetPayStubsFile_ReturnsExcelWithGeneratedPayStub()
    {
        PaidWorker worker = _data.ForFile;
        (await Generate(worker.WorkerId)).EnsureSuccessStatusCode();
        var payStub = await Context.PayStubs.AsNoTracking().SingleAsync(p => p.WorkerProfileId == worker.Profile.Id);

        HttpResponseMessage response = await _client.GetAsync($"{PayStubsController.RouteName}/file?payStubNumber={payStub.PayStubNumber}");

        response.EnsureSuccessStatusCode();
        using var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        IXLWorksheet sheet = workbook.Worksheet("Report");
        Assert.Equal(payStub.PayStubNumber, sheet.Cell("A2").GetString());
        Assert.True(sheet.Cell("A3").IsEmpty());
    }

    [Fact]
    public async Task DeletePayStub_ReleasesTimeSheets_AndDeletesCachedPdf()
    {
        PaidWorker worker = _data.ForDelete;
        (await Generate(worker.WorkerId)).EnsureSuccessStatusCode();
        var payStub = await Context.PayStubs.AsNoTracking().SingleAsync(p => p.WorkerProfileId == worker.Profile.Id);

        HttpResponseMessage response = await _client.DeleteAsync($"{PayStubsController.RouteName}/{payStub.Id}");

        response.EnsureSuccessStatusCode();
        Assert.False(await Context.PayStubs.AnyAsync(p => p.Id == payStub.Id));
        Assert.False(await Context.TimeSheetTotalPayrolls.AnyAsync(t => worker.TimeSheetIds.Contains(t.TimeSheetId)));
        Assert.Contains(await WorkersReady(), w => w.WorkerId == worker.WorkerId);
        Startup.PayStubsContainer.Verify(c => c.DeleteFileIfExists(payStub.Id.ToPayStubBlobName()), Times.Once);
    }

    [Fact]
    public async Task CreatePayStub_Manual_IsSavedWithItemsAndDeductions()
    {
        var model = new CreatePayStubModel
        {
            WorkerProfileId = _data.ForManual.Profile.Id,
            Position = "Forklift",
            WorkBegins = Data.FakeNow,
            WorkEnd = Data.FakeNow.AddDays(3),
            OtherDeductions = 15m,
            OtherDeductionsDescription = "Boots",
            PayVacations = true,
            Items =
            [
                new CreatePayStubItemModel { Type = PayStubItemType.Regular, Quantity = 10, UnitPrice = 20m },
                new CreatePayStubItemModel { Type = PayStubItemType.Other, Quantity = 1, UnitPrice = 50m, Description = "Bonus" }
            ]
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(PayStubsController.RouteName, model);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payStub = await Context.PayStubs.AsNoTracking()
            .Include(p => p.Items)
            .Include(p => p.OtherDeductions)
            .SingleAsync(p => p.WorkerProfileId == _data.ForManual.Profile.Id);
        Assert.Equal("Forklift", payStub.Position);
        Assert.Equal(2, payStub.Items.Count);
        Assert.Equal(250m, payStub.GrossPayment);
        Assert.Equal(10m, payStub.Vacations);
        Assert.Equal(15m, Assert.Single(payStub.OtherDeductions).Total);
        Assert.Equal(DayOfWeek.Sunday, payStub.DateWorkBegins.DayOfWeek);
        Assert.Equal(DayOfWeek.Saturday, payStub.DateWorkEnd.DayOfWeek);
        Assert.True(Assert.Single((await _client.GetFromJsonAsync<PaginatedList<PayStubListModel>>(
            $"{PayStubsController.RouteName}?payStubNumber={payStub.PayStubNumber}")).Items).CanDelete);
    }

    [Fact]
    public async Task CreatePayStub_WithoutItems_ReturnsBadRequest()
    {
        var model = new CreatePayStubModel
        {
            WorkerProfileId = _data.ForManual.Profile.Id,
            WorkBegins = Data.FakeNow,
            WorkEnd = Data.FakeNow.AddDays(3)
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(PayStubsController.RouteName, model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPayStubPdf_UnknownPayStub_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{PayStubsController.RouteName}/{Guid.NewGuid()}/pdf");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SendPayStubEmail_UnknownPayStub_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsync($"{PayStubsController.RouteName}/{Guid.NewGuid()}/email", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SkipPayrollNumber_IsCreated_AndFound()
    {
        const string number = "987654";

        HttpResponseMessage response = await _client.PostAsJsonAsync($"{PayStubsController.RouteName}/skip-payroll-number", new BaseModel<Guid> { Value = number });

        response.EnsureSuccessStatusCode();
        var found = await _client.GetFromJsonAsync<List<BaseModel<Guid>>>($"{PayStubsController.RouteName}/skip-payroll-number?searchTerm={number}");
        Assert.Contains(found, s => s.Value == number);
    }

    [Fact]
    public async Task SkipPayrollNumber_Blank_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{PayStubsController.RouteName}/skip-payroll-number", new BaseModel<Guid> { Value = " " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SendBulkEmail_WithoutPayStubs_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{PayStubsController.RouteName}/email/bulk", new BulkPayStubEmailModel { PayStubIds = [] });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SendBulkEmail_QueuesJobForCurrentAgency()
    {
        Guid[] ids = [Guid.NewGuid(), Guid.NewGuid()];

        HttpResponseMessage response = await _client.PostAsJsonAsync($"{PayStubsController.RouteName}/email/bulk", new BulkPayStubEmailModel { PayStubIds = ids });

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Startup.Publisher.Verify(p => p.SendMessageAsync(
            It.Is<BulkPayStubEmailJob>(j => j.AgencyId == Data.AgencyId && j.PayStubIds.SequenceEqual(ids)),
            It.IsAny<string>(),
            It.IsAny<IReadOnlyDictionary<string, object>>()), Times.Once);
    }

    private static void PayStubItemAssert(Covenant.Common.Entities.Accounting.PayStubs.PayStubItem item, PayStubItemType type, double quantity, decimal unitPrice)
    {
        Assert.Equal(type, item.Type);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(unitPrice, item.UnitPrice);
    }

    public class Startup
    {
        public static readonly Mock<IPayStubsContainer> PayStubsContainer = new();
        public static readonly Mock<IMessagePublisher> Publisher = new();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => o.AddAdminRole(Data.AgencyId));
            services.AddTestDatabase();
            services.AddSingleton(Rates.DefaultRates);
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton<AgencyIdFilter>();
            services.AddSingleton(PayStubsContainer.Object);
            services.AddSingleton(Publisher.Object);
            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            currentUserService.Setup(s => s.GetAgencyIds()).Returns([Data.AgencyId]);
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

    public class PaidWorker
    {
        private readonly Request request;
        private readonly WorkerRequest workerRequest;
        private readonly TimeSheet[] timeSheets;

        public WorkerProfile Profile { get; }
        public Guid WorkerId => Profile.WorkerId;
        public Guid[] TimeSheetIds => [.. timeSheets.Select(t => t.Id)];

        public PaidWorker(Agency agency, CompanyProfile company, City city, string key, bool approved = true)
        {
            Profile = FakeData.FakeWorkerProfile(agency, $"{key}@paystub-worker.com", city);
            request = new Request(company, FakeData.FakeJobPositionRate(company))
            {
                AgencyRate = 30,
                WorkerRate = Data.WorkerRate
            };
            request.UpdateJobLocation(FakeData.FakeLocation(city), false);
            workerRequest = WorkerRequest.AgencyBook(Profile.Id, request.Id);
            timeSheets = [Day(workerRequest, Data.FakeNow, approved), Day(workerRequest, Data.FakeNow.AddDays(1), approved)];
        }

        private static TimeSheet Day(WorkerRequest workerRequest, DateTime date, bool approved)
        {
            if (!approved) return TimeSheet.WorkerClockIn(workerRequest.Id, date.AddHours(8)).Value;
            var timeSheet = TimeSheet.CreateTimeSheet(workerRequest, date, TimeSpan.FromHours(8), now: Data.FakeNow).Value;
            timeSheet.AddApprovedTime(date.AddHours(8), date.AddHours(16));
            return timeSheet;
        }

        public void Seed(CovenantContext context)
        {
            context.WorkerProfiles.Add(Profile);
            context.Requests.Add(request);
            context.WorkerRequests.Add(workerRequest);
            context.TimeSheets.AddRange(timeSheets);
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 08);
        public const decimal WorkerRate = 20m;

        private readonly City city;
        private readonly Agency agency;
        private readonly CompanyProfile company;

        public PaidWorker ForGenerate { get; }
        public PaidWorker ForGenerateTwice { get; }
        public PaidWorker WithoutApprovedTime { get; }
        public PaidWorker ForList { get; }
        public PaidWorker ForFile { get; }
        public PaidWorker ForDelete { get; }
        public PaidWorker ForManual { get; }

        private PaidWorker[] Workers => [ForGenerate, ForGenerateTwice, WithoutApprovedTime, ForList, ForFile, ForDelete, ForManual];

        public Data()
        {
            city = FakeData.FakeCity();
            agency = FakeData.FakeAgency(AgencyId, city);
            company = FakeData.FakeCompanyProfile(agency, city: city, companyEmail: "paystubs@company.com");
            ForGenerate = new PaidWorker(agency, company, city, "generate");
            ForGenerateTwice = new PaidWorker(agency, company, city, "generate-twice");
            WithoutApprovedTime = new PaidWorker(agency, company, city, "not-approved", approved: false);
            ForList = new PaidWorker(agency, company, city, "list");
            ForFile = new PaidWorker(agency, company, city, "file");
            ForDelete = new PaidWorker(agency, company, city, "delete");
            ForManual = new PaidWorker(agency, company, city, "manual", approved: false);
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(city);
            context.CompanyProfiles.Add(company);
            foreach (PaidWorker worker in Workers) worker.Seed(context);
            context.SaveChanges();
        }
    }
}
