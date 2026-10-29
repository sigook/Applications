using Covenant.Api.Controllers.Agencies.Accounting;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoices;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;
using Xunit;
using Covenant.Common.Entities.Agencies;

namespace Covenant.Integration.Tests.Agencies.Accounting;

public class InvoiceDocumentPdfControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public InvoiceDocumentPdfControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    [Fact]
    public async Task GetPdf()
    {
        HttpResponseMessage response = await _client.GetAsync($"{InvoicesController.RouteName}/{Data.Invoice.Id}/pdf");
        response.EnsureSuccessStatusCode();
        byte[] content = await response.Content.ReadAsByteArrayAsync();
        Assert.StartsWith("%PDF", Encoding.ASCII.GetString(content, 0, 4));
        Assert.NotNull((await Context.Invoices.AsNoTracking().SingleAsync(i => i.Id == Data.Invoice.Id)).PdfGeneratedAt);
    }

    [Fact]
    public async Task RegeneratePdf_DiscardsStoredPdf_AndRendersItAgain()
    {
        HttpResponseMessage response = await _client.PostAsync($"{InvoicesController.RouteName}/{Data.RegeneratedInvoice.Id}/pdf/regenerate", null);

        response.EnsureSuccessStatusCode();
        byte[] content = await response.Content.ReadAsByteArrayAsync();
        Assert.StartsWith("%PDF", Encoding.ASCII.GetString(content, 0, 4));
        string blobName = Data.RegeneratedInvoice.Id.ToInvoiceBlobName();
        Startup.InvoicesContainer.Verify(c => c.DeleteFileIfExists(blobName), Times.Once);
        Startup.InvoicesContainer.Verify(c => c.Download(blobName), Times.Never);
        Startup.InvoicesContainer.Verify(c => c.UploadAsync(It.IsAny<byte[]>(), blobName, It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Fact]
    public async Task RegeneratePdf_UnknownInvoice_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.PostAsync($"{InvoicesController.RouteName}/{Guid.NewGuid()}/pdf/regenerate", null);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}

public class Startup
{
    public static readonly Mock<IInvoicesContainer> InvoicesContainer = new();

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDefaultTestConfiguration();
        services.AddTestAuthenticationBuilder()
            .AddTestAuth(o => o.AddAdminRole(Data.Agency.Id));
        services.AddTestDatabase();
        services.AddSingleton(InvoicesContainer.Object);
        services.AddSingleton(Mock.Of<IPayStubsContainer>());
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.Agency.Id);
        currentUserService.Setup(s => s.GetAgencyIds()).Returns(new List<Guid> { Data.Agency.Id });
        services.AddSingleton(currentUserService.Object);

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
    public static readonly Agency Agency = new Agency() { User = FakeData.FakeUser() };
    public static readonly CompanyProfile CompanyProfile = new CompanyProfile { Company = new User(CvnEmail.Create("companyProfile@mail.com").Value), Agency = Agency , Industry = new CompanyProfileIndustry("Test") };
    public static readonly Invoice Invoice = new Invoice
    {
        CompanyProfileId = CompanyProfile.Id,
        InvoiceNumberId = 1,
        InvoiceNumber = "AI-0001-26",
        SubTotal = 1,
        Tax = 1,
        TotalNet = 1
    };
    public static readonly Invoice RegeneratedInvoice = new Invoice
    {
        CompanyProfileId = CompanyProfile.Id,
        InvoiceNumberId = 2,
        InvoiceNumber = "AI-0002-26",
        SubTotal = 1,
        Tax = 1,
        TotalNet = 1,
        PdfGeneratedAt = new DateTime(2026, 1, 1)
    };

    public static void Seed(this CovenantContext context)
    {
        context.Add(CompanyProfile);
        context.AddRange(Invoice, RegeneratedInvoice);
        context.SaveChanges();
    }
}
