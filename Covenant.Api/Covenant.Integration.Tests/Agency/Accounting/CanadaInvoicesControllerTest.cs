using Covenant.Api.Controllers.Agency.Accounting;
using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting.Invoices;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agency.Accounting;

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

    [Fact, TestOrder(1)]
    public async Task Preview_UsesHst()
    {
        var model = new CreateInvoiceModel { CompanyProfileId = _data.CompanyProfile.Id };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{InvoicesController.RouteName}/Preview", model);
        response.EnsureSuccessStatusCode();
        var preview = await response.Content.ReadFromJsonAsync<InvoicePreviewModel>();
        Assert.NotNull(preview);
        Assert.Equal("HST", preview.TaxName);
        Assert.Equal(preview.SubTotal * Rates.DefaultRates.Hst, preview.Tax);
    }

    [Fact, TestOrder(2)]
    public async Task Post_CreatesCanadianInvoice()
    {
        var model = new CreateInvoiceModel { CompanyProfileId = _data.CompanyProfile.Id };
        HttpResponseMessage response = await _client.PostAsJsonAsync(InvoicesController.RouteName, model);
        response.EnsureSuccessStatusCode();

        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var invoice = await context.Invoices.Include(i => i.InvoiceTotals).SingleAsync();
        Assert.StartsWith("AI-", invoice.InvoiceNumber);
        Assert.Equal(_data.TimeSheets.Length, invoice.InvoiceTotals.Count());
        Assert.Equal(invoice.SubTotal * Rates.DefaultRates.Hst, invoice.Tax);
        Assert.Equal(invoice.SubTotal + invoice.Tax, invoice.TotalNet);
    }

    [Fact, TestOrder(3)]
    public async Task GetPayStubs_ReturnsOk()
    {
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var invoice = await context.Invoices.AsNoTracking().SingleAsync();
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
        public static readonly Guid CompanyProfileId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 01);
        public static readonly User CurrentUser = FakeData.FakeUser();

        private readonly City toronto;
        private readonly Request request;

        public Covenant.Common.Entities.Agency.Agency Agency { get; }
        public CompanyProfile CompanyProfile { get; }
        public WorkerProfile Worker { get; }
        public WorkerRequest WorkerRequest { get; }
        public TimeSheet[] TimeSheets { get; }

        public Data()
        {
            toronto = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("CA")));
            Agency = FakeData.FakeAgency(AgencyId, toronto);
            CompanyProfile = FakeData.FakeCompanyProfile(Agency, city: toronto, id: CompanyProfileId);
            Worker = FakeData.FakeWorkerProfile(Agency, "wor@wor.ca", toronto);

            request = new Request(CompanyProfile, FakeData.FakeJobPositionRate(CompanyProfile))
            {
                AgencyRate = 2,
                WorkerRate = 1
            };
            request.UpdateJobLocation(FakeData.FakeLocation(toronto), false);

            WorkerRequest = WorkerRequest.AgencyBook(Worker.Id, request.Id);
            var timeSheet = TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow, TimeSpan.FromHours(8), now: FakeNow).Value;
            var timeSheet1 = TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow.AddDays(1), TimeSpan.FromHours(8), now: FakeNow).Value;
            timeSheet.AddApprovedTime(FakeNow.AddHours(8), FakeNow.AddHours(16));
            timeSheet1.AddApprovedTime(FakeNow.AddDays(1).AddHours(8), FakeNow.AddDays(1).AddHours(16));
            TimeSheets = [timeSheet, timeSheet1];
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(toronto);
            context.Requests.Add(request);
            context.TimeSheets.AddRange(TimeSheets);
            context.WorkerProfiles.Add(Worker);
            context.Users.Add(CurrentUser);
            context.SaveChanges();
        }
    }
}
