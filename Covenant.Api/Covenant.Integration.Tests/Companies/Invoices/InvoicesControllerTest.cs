using Covenant.Api.Controllers.Companies.Invoices;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoices;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http.Json;
using System.Net.Mime;
using Xunit.Abstractions;
using Xunit;
using Covenant.Common.Entities.Agencies;

namespace Covenant.Integration.Tests.Companies.Invoices;

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

    [Fact]
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
        Assert.Equal(Data.PdfUrl(Data.GeneratedInvoice.Id), list.Items.Single(i => i.Id == Data.GeneratedInvoice.Id).PdfUrl);
    }

    [Fact]
    public async Task GetById()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiCompanyInvoice}/{Data.Invoice.Id}");
        response.EnsureSuccessStatusCode();
        InvoiceSummaryModel model = await response.Content.ReadFromJsonAsync<InvoiceSummaryModel>();
        Assert.NotNull(model);
        Assert.Equal(Data.Invoice.InvoiceNumber, model.InvoiceNumber);
        Assert.Equal("HST", model.TaxName);
    }

    [Fact]
    public async Task GetById_OtherCompany_NotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiCompanyInvoice}/{Data.OtherInvoice.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPdf()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiCompanyInvoice}/{Data.Invoice.Id}/pdf");
        response.EnsureSuccessStatusCode();
        Assert.Equal(MediaTypeNames.Application.Pdf, response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(Data.PdfContent, await response.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task GetPdf_NotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiCompanyInvoice}/{Guid.NewGuid()}/pdf");
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
        var invoicesContainer = new Mock<IInvoicesContainer>();
        invoicesContainer.Setup(c => c.Download(Data.Invoice.Id.ToInvoiceBlobName())).ReturnsAsync(Data.PdfContent);
        invoicesContainer.Setup(c => c.GetReadUrl(Data.GeneratedInvoice.Id.ToInvoiceBlobName(), $"Invoice {Data.GeneratedInvoice.InvoiceNumber}.pdf", It.IsAny<DateTimeOffset>()))
            .Returns(Data.PdfUrl(Data.GeneratedInvoice.Id));
        services.AddSingleton(invoicesContainer.Object);
        services.AddSingleton(Mock.Of<IPayStubsContainer>());
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
    public static readonly byte[] PdfContent = "%PDF-1.7"u8.ToArray();
    public static readonly Agency Agency = new Agency() { User = FakeData.FakeUser() };
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
    public static readonly Invoice GeneratedInvoice = new Invoice
    {
        CompanyProfileId = CompanyProfile.Id,
        InvoiceNumberId = 3,
        InvoiceNumber = "AI-0003-26",
        SubTotal = 1,
        Tax = 1,
        TotalNet = 1,
        PdfGeneratedAt = new DateTime(2026, 1, 1)
    };

    public static string PdfUrl(Guid invoiceId) => $"https://blob/invoices/{invoiceId}";

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
        context.AddRange(Invoice, GeneratedInvoice, OtherInvoice);
        context.SaveChanges();
    }
}
