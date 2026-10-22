using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Accounting;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting.Invoices;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Accounting;

public class CanadaInvoicesControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<CanadaInvoicesControllerTest.Startup, CanadaInvoicesControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public CanadaInvoicesControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    [Fact]
    public async Task Preview_UsesHst()
    {
        var model = new CreateInvoiceModel { CompanyProfileId = _data.PreviewCompanyProfile.Id };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{InvoicesController.RouteName}/Preview", model);
        response.EnsureSuccessStatusCode();
        var preview = await response.Content.ReadFromJsonAsync<InvoicePreviewModel>();
        Assert.NotNull(preview);
        Assert.Equal("HST", preview.TaxName);
        Assert.Equal(preview.SubTotal * Rates.DefaultRates.Hst, preview.Tax);
    }

    [Fact]
    public async Task Post_CreatesCanadianInvoice()
    {
        var model = new CreateInvoiceModel { CompanyProfileId = _data.CompanyProfile.Id };
        HttpResponseMessage response = await _client.PostAsJsonAsync(InvoicesController.RouteName, model);
        response.EnsureSuccessStatusCode();

        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var invoice = await context.Invoices.Include(i => i.InvoiceTotals).SingleAsync(i => i.CompanyProfileId == _data.CompanyProfile.Id);
        Assert.StartsWith("AI-", invoice.InvoiceNumber);
        Assert.Equal(_data.TimeSheets.Length, invoice.InvoiceTotals.Count());
        Assert.Equal(invoice.SubTotal * Rates.DefaultRates.Hst, invoice.Tax);
        Assert.Equal(invoice.SubTotal + invoice.Tax, invoice.TotalNet);
    }

    [Fact]
    public async Task GetPayStubs_ReturnsOk()
    {
        var model = new CreateInvoiceModel { CompanyProfileId = _data.PayStubsCompanyProfile.Id };
        HttpResponseMessage created = await _client.PostAsJsonAsync(InvoicesController.RouteName, model);
        created.EnsureSuccessStatusCode();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var invoice = await context.Invoices.AsNoTracking().SingleAsync(i => i.CompanyProfileId == _data.PayStubsCompanyProfile.Id);

        var payStubs = await _client.GetFromJsonAsync<List<PayStubDeleteWarningListModel>>($"{InvoicesController.RouteName}/{invoice.Id}/paystubs");
        Assert.NotNull(payStubs);
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
            services.AddSingleton<IInvoiceRepository, InvoiceRepositoryTest>();
            services.AddSingleton(Rates.DefaultRates);
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton<AgencyIdFilter>();
            services.AddSingleton(Mock.Of<IInvoicesContainer>());
            services.AddSingleton(Mock.Of<IPayStubsContainer>());
            var currentUserService = new Mock<Covenant.Common.Interfaces.ICurrentUserService>();
            currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            currentUserService.Setup(s => s.GetAgencyIds()).Returns(new List<Guid> { Data.AgencyId });
            currentUserService.Setup(s => s.GetUserId()).Returns(Data.CurrentUser.Id);
            services.AddSingleton(currentUserService.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }

        private class InvoiceRepositoryTest : InvoiceRepository
        {
            private static long _id = 1;

            public InvoiceRepositoryTest(CovenantContext context) : base(context)
            {
            }

            public override Task<NextNumberModel> GetNextInvoiceNumber(Guid agencyId) => Task.FromResult(new NextNumberModel { NextNumber = _id++ });
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 01);
        public static readonly User CurrentUser = FakeData.FakeUser();

        private readonly City toronto;
        private readonly BillableCompany post;
        private readonly BillableCompany preview;
        private readonly BillableCompany payStubs;

        public Agency Agency { get; }
        public CompanyProfile CompanyProfile => post.CompanyProfile;
        public CompanyProfile PreviewCompanyProfile => preview.CompanyProfile;
        public CompanyProfile PayStubsCompanyProfile => payStubs.CompanyProfile;
        public TimeSheet[] TimeSheets => post.TimeSheets;

        public Data()
        {
            toronto = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("CA")));
            Agency = FakeData.FakeAgency(AgencyId, toronto);
            post = new BillableCompany(Agency, toronto, "post");
            preview = new BillableCompany(Agency, toronto, "preview");
            payStubs = new BillableCompany(Agency, toronto, "paystubs");
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(toronto);
            context.Users.Add(CurrentUser);
            post.Seed(context);
            preview.Seed(context);
            payStubs.Seed(context);
            context.SaveChanges();
        }

        private class BillableCompany
        {
            private readonly Request request;
            private readonly WorkerProfile worker;

            public CompanyProfile CompanyProfile { get; }
            public TimeSheet[] TimeSheets { get; }

            public BillableCompany(Agency agency, City city, string key)
            {
                CompanyProfile = FakeData.FakeCompanyProfile(agency, city: city, companyEmail: $"{key}@company.ca");
                worker = FakeData.FakeWorkerProfile(agency, $"{key}@worker.ca", city);
                request = new Request(CompanyProfile, FakeData.FakeJobPositionRate(CompanyProfile))
                {
                    AgencyRate = 2,
                    WorkerRate = 1
                };
                request.UpdateJobLocation(FakeData.FakeLocation(city), false);
                var workerRequest = WorkerRequest.AgencyBook(worker.Id, request.Id);
                TimeSheets = [Approved(workerRequest, FakeNow), Approved(workerRequest, FakeNow.AddDays(1))];
            }

            private static TimeSheet Approved(WorkerRequest workerRequest, DateTime date)
            {
                var timeSheet = TimeSheet.CreateTimeSheet(workerRequest, date, TimeSpan.FromHours(8), now: FakeNow).Value;
                timeSheet.AddApprovedTime(date.AddHours(8), date.AddHours(16));
                return timeSheet;
            }

            public void Seed(CovenantContext context)
            {
                context.Requests.Add(request);
                context.TimeSheets.AddRange(TimeSheets);
                context.WorkerProfiles.Add(worker);
            }
        }
    }
}
