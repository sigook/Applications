using ClosedXML.Excel;
using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Accounting;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoices;
using Covenant.Common.Entities.Accounting.PayStubs;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting.Invoices;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Accounting;

public class ReportsControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ReportsControllerTest.Startup, ReportsControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public ReportsControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    private static string Range(DateTime from, DateTime to) => $"startDate={from:yyyy-MM-dd}&endDate={to:yyyy-MM-dd}";

    private static async Task<IXLWorksheet> Sheet(HttpResponseMessage response, string name = null)
    {
        response.EnsureSuccessStatusCode();
        var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        return name is null ? workbook.Worksheet(1) : workbook.Worksheet(name);
    }

    private async Task Invoice(CompanyProfile company)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(InvoicesController.RouteName, new CreateInvoiceModel { CompanyProfileId = company.Id });
        response.EnsureSuccessStatusCode();
    }

    private Task<HoursWorkedResume> HoursWorked(CompanyProfile company) =>
        _client.GetFromJsonAsync<HoursWorkedResume>(
            $"{ReportsController.RouteName}/hours-worked?{Range(Data.Monday, Data.Monday.AddDays(6))}&companyProfileId={company.Id}");

    [Fact]
    public async Task HoursWorked_SumsInvoicedHoursByWorkerAndBillRate()
    {
        await Invoice(_data.WholeHours.Company);

        HoursWorkedResume report = await HoursWorked(_data.WholeHours.Company);

        HoursWorkedResponse row = Assert.Single(report.Detail);
        Assert.Equal("Hana Hours", row.WorkerName);
        Assert.Equal("Forklift", row.JobPosition);
        Assert.Equal(Data.BillRate, row.BillRate);
        Assert.Equal(16, row.RegularHoursWorked);
        Assert.Equal(16 * Data.BillRate, row.TotalPayRegularRate);
        Assert.Equal(16, report.TotalHours);
        Assert.Equal(16 * Data.BillRate, report.TotalPay);
    }

    [Fact]
    public async Task HoursWorked_FractionalHours_ArePaidInFull()
    {
        await Invoice(_data.HalfHours.Company);

        HoursWorkedResume report = await HoursWorked(_data.HalfHours.Company);

        HoursWorkedResponse row = Assert.Single(report.Detail);
        Assert.Equal(7.5, row.RegularHoursWorked);
        Assert.Equal(7.5m * Data.BillRate, row.TotalPayRegularRate);
    }

    [Fact]
    public async Task HoursWorkedFile_ReturnsOneRowPerWorker()
    {
        await Invoice(_data.ForFile.Company);

        IXLWorksheet sheet = await Sheet(await _client.GetAsync(
            $"{ReportsController.RouteName}/hours-worked/file?{Range(Data.Monday, Data.Monday.AddDays(6))}&companyProfileId={_data.ForFile.Company.Id}"));

        Assert.Equal("Fiona File", sheet.Cell("A2").GetString());
        Assert.True(sheet.Cell("A3").IsEmpty());
    }

    [Fact]
    public async Task TimesheetsFile_ListsTheAgencyTimesheetsInTheRange()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ReportsController.RouteName}/timesheets/file?{Range(Data.Monday, Data.Monday.AddDays(6))}");

        IXLWorksheet sheet = await Sheet(response);
        Assert.True(sheet.RowsUsed().Count() > 1);
    }

    [Fact]
    public async Task JobPositions_ReturnsTheClientPositionsWorkedInTheRange()
    {
        var positions = await _client.GetFromJsonAsync<List<CompanyProfileJobPositionRateModel>>(
            $"{ReportsController.RouteName}/{_data.WholeHours.Company.Id}/job-positions?{Range(Data.Monday, Data.Monday.AddDays(6))}");

        Assert.Contains(positions, p => p.JobPosition == "Forklift");
    }

    [Theory]
    [InlineData("t4")]
    [InlineData("cra-payroll")]
    public async Task PayrollFiles_WithoutDates_ReturnBadRequest(string report)
    {
        HttpResponseMessage response = await _client.GetAsync($"{ReportsController.RouteName}/{report}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("t4")]
    [InlineData("cra-payroll")]
    public async Task PayrollFiles_WithoutPayStubsInTheRange_ReturnBadRequest(string report)
    {
        HttpResponseMessage response = await _client.GetAsync($"{ReportsController.RouteName}/{report}?{Range(new DateTime(2001, 1, 1), new DateTime(2001, 12, 31))}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task T4_ListsEachWorkerWithTheirYearTotals()
    {
        IXLWorksheet report = await Sheet(await _client.GetAsync($"{ReportsController.RouteName}/t4?{Range(Data.PayYearStart, Data.PayYearEnd)}"), "Report");

        Assert.Equal("Paula Payroll", report.Cell("B2").GetString());
        Assert.Equal("123456789", report.Cell("D2").GetString());
        Assert.Equal(Data.PayStubs.Sum(p => p.TotalEarnings), report.Cell("E2").GetValue<decimal>());
    }

    [Fact]
    public async Task CraPayroll_ListsOneRowPerPayStub()
    {
        IXLWorksheet sheet = await Sheet(await _client.GetAsync($"{ReportsController.RouteName}/cra-payroll?{Range(Data.PayYearStart, Data.PayYearEnd)}"), "Report");

        Assert.Equal("Paula Payroll", sheet.Cell("B4").GetString());
        Assert.Equal(Data.PayStubs.Select(p => p.PayStubNumber).Order(), new[] { sheet.Cell("D4").GetString(), sheet.Cell("D5").GetString() }.Order());
        Assert.True(sheet.Cell("D6").IsEmpty());
    }

    [Fact]
    public async Task Payments_GroupsPayStubsByPaymentDate()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WeeklyPayrollModel>>($"{ReportsController.RouteName}/payments?pageSize=50");

        foreach (PayStub payStub in Data.PayStubs)
        {
            WeeklyPayrollModel week = Assert.Single(page.Items, w => w.WeekEnding.Date == payStub.PaymentDate.Date);
            Assert.Equal(1, week.NumberOfPayStubs);
            Assert.Equal(payStub.TotalPaid, week.TotalNet);
        }
    }

    [Fact]
    public async Task PaymentsFile_ForAPaymentDate_ReturnsExcel()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ReportsController.RouteName}/payments/file?weekEnding={Data.PayStubs[0].PaymentDate:yyyy-MM-dd}");

        IXLWorksheet sheet = await Sheet(response);
        Assert.True(sheet.RowsUsed().Count() > 1);
    }

    [Fact]
    public async Task PaymentsFile_InvalidDate_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ReportsController.RouteName}/payments/file?weekEnding=not-a-date");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => o.AddAdminRole(Data.AgencyId));
            services.AddTestDatabase();
            services.AddSingleton<IInvoiceRepository, InvoiceRepositoryTest>();
            services.AddSingleton(Rates.DefaultRates);
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton<AgencyIdFilter>();
            services.AddSingleton(Mock.Of<IInvoicesContainer>());
            services.AddSingleton(Mock.Of<IPayStubsContainer>());
            services.AddSingleton(Mock.Of<ITeamsService>());
            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            currentUserService.Setup(s => s.GetAgencyIds()).Returns([Data.AgencyId]);
            currentUserService.Setup(s => s.GetUserId()).Returns(Data.CurrentUser.Id);
            services.AddSingleton(currentUserService.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}"));
        }

        private class InvoiceRepositoryTest(CovenantContext context) : InvoiceRepository(context)
        {
            private static long _id = 1;

            public override Task<NextNumberModel> GetNextInvoiceNumber(Guid agencyId) => Task.FromResult(new NextNumberModel { NextNumber = _id++ });
        }
    }

    public class WorkedClient
    {
        private readonly LocationTax locationTax;
        private readonly Request request;
        private readonly WorkerProfile worker;
        private readonly TimeSheet[] timeSheets;

        public CompanyProfile Company { get; }

        public WorkedClient(Agency agency, City city, string firstName, string lastName, params double[] hoursPerDay)
        {
            string key = $"{firstName}.{lastName}".ToLower();
            Company = FakeData.FakeCompanyProfile(agency, city: city, companyEmail: $"{key}@reports-client.com");
            worker = FakeData.FakeWorkerProfile(agency, $"{key}@reports-worker.com", city)
                .With(w => { w.FirstName = firstName; w.LastName = lastName; });
            var jobLocation = FakeData.FakeLocation(city);
            locationTax = new LocationTax { LocationId = jobLocation.Id, Tax1 = 0.13m };
            request = new Request(Company, FakeData.FakeJobPositionRate(Company, "Forklift", Data.BillRate, 20)) { AgencyRate = Data.BillRate, WorkerRate = 20 };
            request.UpdateJobLocation(jobLocation, false);
            var workerRequest = WorkerRequest.AgencyBook(worker.Id, request.Id);
            timeSheets = [.. hoursPerDay.Select((hours, day) => Approved(workerRequest, Data.Monday.AddDays(day), hours))];
        }

        private static TimeSheet Approved(WorkerRequest workerRequest, DateTime date, double hours)
        {
            var timeSheet = TimeSheet.CreateTimeSheet(workerRequest, date, TimeSpan.FromHours(hours), now: Data.Monday).Value;
            timeSheet.AddApprovedTime(date.AddHours(8), date.AddHours(8 + hours));
            return timeSheet;
        }

        public void Seed(CovenantContext context)
        {
            context.LocationTaxes.Add(locationTax);
            context.WorkerProfiles.Add(worker);
            context.Requests.Add(request);
            context.TimeSheets.AddRange(timeSheets);
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly DateTime Monday = new(2019, 01, 07);
        public static readonly DateTime PayYearStart = new(2018, 01, 01);
        public static readonly DateTime PayYearEnd = new(2018, 12, 31);
        public const decimal BillRate = 30m;
        public static readonly User CurrentUser = FakeData.FakeUser("accounting@reports.com");

        private readonly City city;
        private readonly Agency agency;
        private readonly WorkerProfile payrollWorker;

        public WorkedClient WholeHours { get; }
        public WorkedClient HalfHours { get; }
        public WorkedClient ForFile { get; }
        public static PayStub[] PayStubs { get; private set; }

        public Data()
        {
            city = FakeData.FakeCity();
            agency = FakeData.FakeAgency(AgencyId, city);
            WholeHours = new WorkedClient(agency, city, "Hana", "Hours", 8, 8);
            HalfHours = new WorkedClient(agency, city, "Hal", "Half", 7.5);
            ForFile = new WorkedClient(agency, city, "Fiona", "File", 8);
            payrollWorker = FakeData.FakeWorkerProfile(agency, "paula.payroll@reports-worker.com", city)
                .With(w => { w.FirstName = "Paula"; w.LastName = "Payroll"; w.SocialInsurance = "123456789"; });
            PayStubs = [PayStubFor(payrollWorker, 1, new DateTime(2018, 03, 09), 500m), PayStubFor(payrollWorker, 2, new DateTime(2018, 03, 16), 700m)];
        }

        private static PayStub PayStubFor(WorkerProfile worker, long number, DateTime paymentDate, decimal earnings)
        {
            return PayStub.Create(
                workerProfileId: worker.Id,
                payStubNumberId: number,
                payStubNumber: $"PS-{number:0000}-18",
                position: "Forklift",
                dateWorkBegins: paymentDate.AddDays(-12),
                dateWorkEnd: paymentDate.AddDays(-6),
                paymentDate: paymentDate,
                weekEnding: paymentDate.AddDays(-6),
                createdAt: paymentDate,
                regularWage: earnings,
                grossPayment: earnings,
                vacations: 0,
                totalEarnings: earnings,
                cpp: 20m,
                ei: 8m,
                federalTax: 40m,
                provincialTax: 10m,
                totalDeductions: 78m,
                totalPaid: earnings - 78m,
                items: [PayStubItem.CreateRegular(20, earnings / 20).Value],
                otherDeductions: []).With(p => p.NumberId = number);
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(city);
            context.Users.Add(CurrentUser);
            context.AgencyPersonnel.Add(AgencyPersonnel.CreatePrimary(AgencyId, CurrentUser.Id, "Accounting"));
            WholeHours.Seed(context);
            HalfHours.Seed(context);
            ForFile.Seed(context);
            context.WorkerProfiles.Add(payrollWorker);
            context.PayStubs.AddRange(PayStubs);
            context.SaveChanges();
        }
    }
}
