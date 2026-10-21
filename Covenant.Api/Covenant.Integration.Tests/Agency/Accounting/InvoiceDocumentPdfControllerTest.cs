using Covenant.Api.Controllers.Agency.Accounting;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;
using Xunit;

namespace Covenant.Integration.Tests.Agency.Accounting;

public class InvoiceDocumentPdfControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    public InvoiceDocumentPdfControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetPdf()
    {
        HttpResponseMessage response = await _client.GetAsync($"{InvoicesController.RouteName}/{Data.Invoice.Id}/pdf");
        response.EnsureSuccessStatusCode();
        byte[] content = await response.Content.ReadAsByteArrayAsync();
        Assert.StartsWith("%PDF", Encoding.ASCII.GetString(content, 0, 4));
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDefaultTestConfiguration();
        services.AddTestAuthenticationBuilder()
            .AddTestAuth(o => o.AddAdminRole(Data.Agency.Id));
        services.AddTestDatabase();
        services.AddSingleton(Mock.Of<IInvoicesContainer>());
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
    public static readonly Covenant.Common.Entities.Agency.Agency Agency = new Covenant.Common.Entities.Agency.Agency() { User = FakeData.FakeUser() };
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

    public static void Seed(this CovenantContext context)
    {
        context.Add(CompanyProfile);
        context.AddRange(Invoice);
        context.SaveChanges();
    }
}
