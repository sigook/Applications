using Covenant.Api.Controllers.Agency.Accounting;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agency.Accounting;

public class DeleteSubcontractorReportControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<DeleteSubcontractorReportControllerTest.Startup, DeleteSubcontractorReportControllerTest.Data>>
{
    private const string Url = ReportsController.RouteName + "/subcontractors";

    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public DeleteSubcontractorReportControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    [Fact]
    public async Task DeleteReturnsBadRequestWhenTheDateIsInvalid()
    {
        int before = await Context.ReportSubcontractors.CountAsync();

        HttpResponseMessage response = await _client.DeleteAsync($"{Url}?weekEnding=not-a-date");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await Context.ReportSubcontractors.CountAsync());
    }

    [Fact]
    public async Task DeleteReturnsBadRequestWhenTheWeekHasNoReports()
    {
        int before = await Context.ReportSubcontractors.CountAsync();

        HttpResponseMessage response = await _client.DeleteAsync($"{Url}?weekEnding=2019-02-02");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await Context.ReportSubcontractors.CountAsync());
    }

    [Fact]
    public async Task DeleteRemovesTheWeekReportsAndReleasesTheirTimesheets()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{Url}?weekEnding={Data.WeekOne:yyyy-MM-dd}");

        response.EnsureSuccessStatusCode();

        var context = Context;
        List<ReportSubcontractor> reports = await context.ReportSubcontractors.AsNoTracking().ToListAsync();
        Assert.DoesNotContain(reports, r => r.Id == _data.ReportWeekOne.Id);
        Assert.Contains(reports, r => r.Id == _data.ReportWeekTwo.Id);
        Assert.Contains(reports, r => r.Id == _data.ReportOtherAgency.Id);

        List<ReportSubcontractorWageDetail> wageDetails = await context.ReportSubcontractorWageDetails.AsNoTracking().ToListAsync();
        Assert.DoesNotContain(wageDetails, w => w.ReportSubcontractorId == _data.ReportWeekOne.Id);
        Assert.Contains(wageDetails, w => w.ReportSubcontractorId == _data.ReportWeekTwo.Id);

        Guid[] releasedTimeSheetIds = [_data.TimeSheets[0].Id, _data.TimeSheets[1].Id];
        List<TimeSheetTotalPayroll> payrollTotals = await context.TimeSheetTotalPayrolls.AsNoTracking().ToListAsync();
        Assert.DoesNotContain(payrollTotals, p => releasedTimeSheetIds.Contains(p.TimeSheetId));
        Assert.Contains(payrollTotals, p => p.TimeSheetId == _data.TimeSheets[2].Id);

        Assert.Equal(_data.TimeSheets.Length, await context.TimeSheets.CountAsync());
    }

    [Fact]
    public async Task DeleteReturnsBadRequestWhenTheWeekWasAlreadyDeleted()
    {
        HttpResponseMessage first = await _client.DeleteAsync($"{Url}?weekEnding={Data.WeekThree:yyyy-MM-dd}");
        first.EnsureSuccessStatusCode();

        HttpResponseMessage response = await _client.DeleteAsync($"{Url}?weekEnding={Data.WeekThree:yyyy-MM-dd}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o =>
            {
                o.AddAdminRole(Data.AgencyId);
            });
            services.AddTestDatabase();
            services.AddSingleton<AgencyIdFilter>();
            services.AddSingleton(Mock.Of<IPayStubsContainer>());
            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            currentUserService.Setup(s => s.GetAgencyIds()).Returns(new List<Guid> { Data.AgencyId });
            services.AddSingleton(currentUserService.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 01);
        public static readonly DateTime WeekOne = new(2019, 01, 05);
        public static readonly DateTime WeekTwo = new(2019, 01, 12);
        public static readonly DateTime WeekThree = new(2019, 01, 19);

        private readonly City city;
        private readonly Request request;
        private readonly Covenant.Common.Entities.Agency.Agency otherAgency;

        public CompanyProfile CompanyProfile { get; }
        public WorkerProfile Worker { get; }
        public WorkerProfile OtherAgencyWorker { get; }
        public WorkerRequest WorkerRequest { get; }
        public TimeSheet[] TimeSheets { get; }
        public ReportSubcontractor ReportWeekOne { get; }
        public ReportSubcontractor ReportWeekTwo { get; }
        public ReportSubcontractor ReportWeekThree { get; }
        public ReportSubcontractor ReportOtherAgency { get; }

        public Data()
        {
            city = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry()));
            var agency = FakeData.FakeAgency(AgencyId, city);
            CompanyProfile = FakeData.FakeCompanyProfile(agency, city: city);
            Worker = FakeData.FakeWorkerProfile(agency, "subcontractor@test.com", city);
            Worker.IsSubcontractor = true;

            request = new Request(CompanyProfile, FakeData.FakeJobPositionRate(CompanyProfile))
            {
                AgencyRate = 2,
                WorkerRate = 1
            };
            request.UpdateJobLocation(FakeData.FakeLocation(city), false);

            WorkerRequest = WorkerRequest.AgencyBook(Worker.Id, request.Id);
            TimeSheets =
            [
                ApprovedTimeSheet(FakeNow),
                ApprovedTimeSheet(FakeNow.AddDays(1)),
                ApprovedTimeSheet(FakeNow.AddDays(7)),
                ApprovedTimeSheet(FakeNow.AddDays(14))
            ];

            ReportWeekOne = Report(Worker.Id, WeekOne, 1, [WageDetail(TimeSheets[0]), WageDetail(TimeSheets[1])]);
            ReportWeekTwo = Report(Worker.Id, WeekTwo, 2, [WageDetail(TimeSheets[2])]);
            ReportWeekThree = Report(Worker.Id, WeekThree, 4, [WageDetail(TimeSheets[3])]);

            otherAgency = FakeData.FakeAgency(city: city);
            OtherAgencyWorker = FakeData.FakeWorkerProfile(otherAgency, "other.subcontractor@test.com", city);
            OtherAgencyWorker.IsSubcontractor = true;
            ReportOtherAgency = Report(OtherAgencyWorker.Id, WeekOne, 3, []);
        }

        private TimeSheet ApprovedTimeSheet(DateTime date)
        {
            var timeSheet = TimeSheet.CreateTimeSheet(WorkerRequest, date, TimeSpan.FromHours(8), now: FakeNow).Value;
            timeSheet.AddApprovedTime(date.AddHours(8), date.AddHours(16));
            return timeSheet;
        }

        private static ReportSubcontractorWageDetail WageDetail(TimeSheet timeSheet) =>
            new(workerRate: 1, regular: 8, otherRegular: 0, missing: 0, missingOvertime: 0, nightShift: 0, holiday: 0, overtime: 0)
            {
                TimeSheetTotal = TimeSheetTotalPayroll.CreateTotal(
                    timeSheet.Id,
                    TimeSpan.FromHours(8),
                    TimeSpan.FromHours(8),
                    TimeSpan.Zero,
                    TimeSpan.Zero,
                    TimeSpan.FromHours(8))
            };

        private static ReportSubcontractor Report(Guid workerProfileId, DateTime weekEnding, long numberId, ReportSubcontractorWageDetail[] wageDetails)
        {
            var report = new ReportSubcontractor
            {
                WorkerProfileId = workerProfileId,
                RegularWage = 8 * wageDetails.Length,
                Gross = 8 * wageDetails.Length,
                Earnings = 8 * wageDetails.Length,
                TotalNet = 8 * wageDetails.Length,
                DateWorkBegins = weekEnding.AddDays(-6),
                DateWorkEnd = weekEnding,
                WeekEnding = weekEnding,
                NumberId = numberId
            };
            report.AddWageDetail(wageDetails);
            return report;
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(city);
            context.Requests.Add(request);
            context.WorkerProfiles.AddRange(Worker, OtherAgencyWorker);
            context.TimeSheets.AddRange(TimeSheets);
            context.ReportSubcontractors.AddRange(ReportWeekOne, ReportWeekTwo, ReportWeekThree, ReportOtherAgency);
            context.SaveChanges();
        }
    }
}
