using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Accounting;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Covenant.Integration.Tests.CompanyModule.CompanyInvoice
{
    public class TestCompanyInvoiceController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private const string ApiCompanyInvoice = "api/CompanyInvoice";
        private readonly HttpClient _client;
        public TestCompanyInvoiceController(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _output = output;
            _client = factory.CreateClient();
        }

        [Fact, TestOrder(1)]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(ApiCompanyInvoice);
            _output.WriteLine("*********************Before read the response");
            if (!response.IsSuccessStatusCode)
            {
                _output.WriteLine(await response.Content.ReadAsStringAsync());
            }
            _output.WriteLine("*********************After read the response");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<InvoiceListModel>>();
            Assert.NotEmpty(list.Items);
        }

        [Fact, TestOrder(2)]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{ApiCompanyInvoice}/{Data.Invoice.Id}");
            response.EnsureSuccessStatusCode();
            InvoiceSummaryModel model = await response.Content.ReadAsJsonAsync<InvoiceSummaryModel>();
            Assert.NotNull(model);
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddSub(Data.CompanyProfile.Company.Id);
                    o.AddCompanyRole();
                });
            services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
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
            context.Seed();
        }
    }

    public static class Data
    {
        public static readonly Covenant.Common.Entities.Agency.Agency Agency = new Covenant.Common.Entities.Agency.Agency();
        public static readonly CompanyProfile CompanyProfile = new CompanyProfile
        {
            Company = new User(CvnEmail.Create("comp@a.com").Value),
            Agency = Agency,
            Locations = new List<CompanyProfileLocation>
            {
                new CompanyProfileLocation
                {
                    IsBilling = true,
                    Location = FakeData.FakeLocation()
                }
            },
            Industry = CompanyProfileIndustry.Create("Test").Value,
            Logo = new CovenantFile("test.png")
        };
        public static readonly Invoice Invoice = new Invoice(CompanyProfile.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

        public static void Seed(this CovenantContext context)
        {
            context.CompanyProfile.Add(CompanyProfile);
            context.AddRange(Invoice);
            context.SaveChanges();
        }
    }
}