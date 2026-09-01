using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.AccountingModule.AgencyInvoice;

public class AccountingInvoiceControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<AccountingInvoiceControllerTest.Startup, AccountingInvoiceControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public AccountingInvoiceControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    [Fact, TestOrder(1)]
    public async Task Preview()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyProfileId = _data.CompanyProfile.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"api/agency/accounting/Invoices/Preview", model);
        response.EnsureSuccessStatusCode();
        var preview = await response.Content.ReadFromJsonAsync<InvoicePreviewModel>();
        Assert.NotNull(preview);
        Assert.NotEmpty(preview.Items);
        Assert.NotEmpty(preview.Discounts);
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        Assert.Empty(await context.InvoicesUSA.ToListAsync());
    }

    [Fact, TestOrder(2)]
    public async Task Post()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyProfileId = _data.CompanyProfile.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/agency/accounting/Invoices", model);
        response.EnsureSuccessStatusCode();
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        Assert.Single(await context.InvoicesUSA.ToListAsync());
        Assert.Equal(_data.TimeSheets.Length, await context.TimeSheetTotals.CountAsync());
        Assert.Equal(3, (await context.InvoicesUSA.SingleAsync()).Items.Count());
        Assert.Equal(model.Discounts.Count(), (await context.InvoicesUSA.SingleAsync()).Discounts.Count());
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
            var invoiceContainer = new Mock<IInvoicesContainer>();
            services.AddSingleton(invoiceContainer.Object);
            var payStubContainer = new Mock<IPayStubsContainer>();
            services.AddSingleton(payStubContainer.Object);
            var identityServerService = new Mock<Covenant.Common.Interfaces.IIdentityServerService>();
            identityServerService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            identityServerService.Setup(s => s.GetAgencyIds()).Returns(new List<Guid> { Data.AgencyId });
            services.AddSingleton(identityServerService.Object);
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

            public override Task<NextNumberModel> GetNextInvoiceUSANumber() => Task.FromResult(new NextNumberModel { NextNumber = _id++ });
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly Guid CompanyProfileId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 01);

        private readonly City toronto;
        private readonly LocationTax locationTax;
        private readonly Request request;

        public Covenant.Common.Entities.Agency.Agency Agency { get; }
        public CompanyProfile CompanyProfile { get; }
        public WorkerProfile Worker { get; }
        public WorkerRequest WorkerRequest { get; }
        public TimeSheet[] TimeSheets { get; }

        public Data()
        {
            toronto = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("USA")));
            Agency = FakeData.FakeAgency(AgencyId, toronto);
            CompanyProfile = FakeData.FakeCompanyProfile(Agency, city: toronto, id: CompanyProfileId);
            Worker = FakeData.FakeWorkerProfile(Agency, "wor@wor.com", toronto);

            var jobLocation = FakeData.FakeLocation(toronto);
            locationTax = new LocationTax { LocationId = jobLocation.Id, Tax1 = 0.06m };

            request = new Request(CompanyProfile, FakeData.FakeJobPositionRate(CompanyProfile))
            {
                AgencyRate = 2,
                WorkerRate = 1
            };
            request.UpdateJobLocation(jobLocation, false);

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
            context.LocationTaxes.Add(locationTax);
            context.Requests.Add(request);
            context.TimeSheets.AddRange(TimeSheets);
            context.WorkerProfiles.Add(Worker);
            context.SaveChanges();
        }
    }
}
