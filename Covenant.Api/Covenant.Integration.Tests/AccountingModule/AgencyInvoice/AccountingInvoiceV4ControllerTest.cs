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
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Accounting;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.AccountingModule.AgencyInvoice;

public class AccountingInvoiceV4ControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AccountingInvoiceV4ControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public AccountingInvoiceV4ControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact, TestOrder(1)]
    public async Task Preview()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyId = Data.FakeCompany.Company.Id,
            CompanyProfileId = Data.FakeCompany.Id,
            ProvinceId = Data.Toronto.Province.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"api/agency/accounting/Invoices/Preview", model);
        response.EnsureSuccessStatusCode();
        var preview = await response.Content.ReadAsJsonAsync<InvoicePreviewModel>();
        Assert.NotNull(preview);
        Assert.NotEmpty(preview.Items);
        Assert.NotEmpty(preview.Discounts);
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        Assert.Empty(await context.InvoiceUSA.ToListAsync());
    }

    [Fact, TestOrder(2)]
    public async Task Post()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyId = Data.FakeCompany.Company.Id,
            CompanyProfileId = Data.FakeCompany.Id,
            ProvinceId = Data.Toronto.Province.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/agency/accounting/Invoices", model);
        response.EnsureSuccessStatusCode();
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        Assert.Single(await context.InvoiceUSA.ToListAsync());
        Assert.Equal(Data.TimeSheets.Length, await context.TimeSheetTotal.CountAsync());
        Assert.Equal(3, (await context.InvoiceUSA.SingleAsync()).Items.Count());
        Assert.Equal(model.Discounts.Count(), (await context.InvoiceUSA.SingleAsync()).Discounts.Count());
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o =>
            {
                o.AddAgencyPersonnelRole(Data.FakeAgency.Id);
            });
            services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            services.AddSingleton<IInvoiceRepository, InvoiceRepositoryTest>();
            services.AddSingleton(Rates.DefaultRates);
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton<AgencyIdFilter>();
            var invoiceContainer = new Mock<IInvoicesContainer>();
            services.AddSingleton(invoiceContainer.Object);
            var payStubContainer = new Mock<IPayStubsContainer>();
            services.AddSingleton(payStubContainer.Object);
            var identityServerService = new Mock<Covenant.Common.Interfaces.IIdentityServerService>();
            identityServerService.Setup(s => s.GetAgencyId()).Returns(Data.FakeAgency.Id);
            identityServerService.Setup(s => s.GetAgencyIds()).Returns(new List<Guid> { Data.FakeAgency.Id });
            services.AddSingleton(identityServerService.Object);
        }

        public void Configure(IApplicationBuilder app, CovenantContext context)
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
            Data.Seed(context);
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

    private static class Data
    {
        public static readonly DateTime FakeNow = new DateTime(2019, 01, 01);
        public static readonly City Toronto = new City { Province = new Province { Country = new Country { Code = "USA" } } };
        public static readonly ProvinceTax ProvinceTax = new ProvinceTax { ProvinceId = Toronto.Province.Id, Tax1 = 0.06m };
        public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency
        {
            User = new User(CvnEmail.Create("test@test.com").Value)
        };
        public static readonly WorkerProfile FakeWorker = new WorkerProfile(new User(CvnEmail.Create("wor@wor.com").Value)) { Agency = FakeAgency };
        public static readonly CompanyProfile FakeCompany = new CompanyProfile
        {
            Company = new User(CvnEmail.Create("com@com.com").Value),
            Agency = FakeAgency,
            Industry = new CompanyProfileIndustry("Test"),
        };

        private static readonly Request FakeRequest = new Request(FakeCompany.Company, FakeAgency, new CompanyProfileJobPositionRate { CompanyProfile = FakeCompany })
        {
            AgencyRate = 2,
            WorkerRate = 1
        };

        public static readonly WorkerRequest FakeWorkerRequest = WorkerRequest.AgencyBook(FakeWorker.WorkerId, FakeRequest.Id);
        public static readonly TimeSheet TimeSheet = TimeSheet.CreateTimeSheet(FakeWorkerRequest, FakeNow, TimeSpan.FromHours(8), now: FakeNow).Value;
        public static readonly TimeSheet TimeSheet1 = TimeSheet.CreateTimeSheet(FakeWorkerRequest, FakeNow.AddDays(1), TimeSpan.FromHours(8), now: FakeNow).Value;
        public static readonly TimeSheet[] TimeSheets = { TimeSheet, TimeSheet1 };

        public static void Seed(CovenantContext context)
        {
            FakeAgency.AddLocation(Location.Create(Toronto.Id, "424 Dundas", "M3P1M7").Value, true);
            FakeRequest.UpdateJobLocation(Location.Create(Toronto.Id, "424 Dundas", "M3P1M7").Value, false);
            FakeCompany.AddLocation(Location.Create(Toronto.Id, "424 Dundas", "M3P1M7").Value, false);

            // Approve timesheets
            TimeSheet.AddApprovedTime(FakeNow.AddHours(8), FakeNow.AddHours(16));
            TimeSheet1.AddApprovedTime(FakeNow.AddDays(1).AddHours(8), FakeNow.AddDays(1).AddHours(16));

            context.City.Add(Toronto);
            context.ProvinceTaxes.Add(ProvinceTax);
            context.Request.Add(FakeRequest);
            context.TimeSheet.AddRange(TimeSheets);
            context.WorkerProfile.Add(FakeWorker);
            context.SaveChanges();
        }
    }
}