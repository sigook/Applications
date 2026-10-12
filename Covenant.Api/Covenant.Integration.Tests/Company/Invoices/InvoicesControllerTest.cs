using Covenant.Api.Controllers.Company.Invoices;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit.Abstractions;
using Xunit;

namespace Covenant.Integration.Tests.Company.Invoices;

public class InvoicesControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly ITestOutputHelper _output;
    private const string ApiCompanyInvoice = InvoicesController.RouteName;
    private readonly HttpClient _client;
    public InvoicesControllerTest(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
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
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<InvoiceListModel>>();
        Assert.NotEmpty(list.Items);
    }

    [Fact, TestOrder(2)]
    public async Task GetById()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiCompanyInvoice}/{Data.Invoice.Id}");
        response.EnsureSuccessStatusCode();
        InvoiceSummaryModel model = await response.Content.ReadFromJsonAsync<InvoiceSummaryModel>();
        Assert.NotNull(model);
        Assert.Equal(Data.Invoice.InvoiceNumber, model.InvoiceNumber);
        Assert.Equal("HST", model.TaxName);
    }

    [Fact, TestOrder(3)]
    public async Task GetById_OtherCompany_NotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiCompanyInvoice}/{Data.OtherInvoice.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
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
        services.AddTestDatabase();
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
    public static readonly Covenant.Common.Entities.Agency.Agency Agency = new Covenant.Common.Entities.Agency.Agency() { User = FakeData.FakeUser() };
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
    public static readonly Invoice Invoice = new Invoice
    {
        CompanyProfileId = CompanyProfile.Id,
        InvoiceNumberId = 1,
        InvoiceNumber = "AI-0001-26",
        SubTotal = 1,
        Tax = 1,
        TotalNet = 1
    };

    public static readonly CompanyProfile OtherCompanyProfile = new CompanyProfile
    {
        Company = new User(CvnEmail.Create("other@a.com").Value),
        Agency = Agency,
        Industry = CompanyProfileIndustry.Create("Test").Value
    };
    public static readonly Invoice OtherInvoice = new Invoice
    {
        CompanyProfileId = OtherCompanyProfile.Id,
        InvoiceNumberId = 2,
        InvoiceNumber = "AI-0002-26",
        SubTotal = 1,
        Tax = 1,
        TotalNet = 1
    };

    public static void Seed(this CovenantContext context)
    {
        context.CompanyProfiles.AddRange(CompanyProfile, OtherCompanyProfile);
        context.AddRange(Invoice, OtherInvoice);
        context.SaveChanges();
    }
}
