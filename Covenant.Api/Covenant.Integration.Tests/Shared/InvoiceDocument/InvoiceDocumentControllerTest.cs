using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Pdf;
using Covenant.Common.Repositories.Company;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.Shared.InvoiceDocument
{
    public class InvoiceDocumentControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public InvoiceDocumentControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task GetPdf()
        {
            HttpResponseMessage response = await _client.GetAsync($"api/agency/accounting/Invoices/{Data.Invoice.Id}/pdf");
            response.EnsureSuccessStatusCode();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o => { });
            services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            services.AddSingleton(Mock.Of<IInvoicesContainer>());
            services.AddSingleton(Mock.Of<IPayStubsContainer>());
            var identityServerService = new Mock<IIdentityServerService>();
            identityServerService.Setup(s => s.GetAgencyId()).Returns(Data.Agency.Id);
            identityServerService.Setup(s => s.GetAgencyIds()).Returns(new List<Guid> { Data.Agency.Id });
            services.AddSingleton(identityServerService.Object);
            var pdfService = new Mock<IPdfGeneratorService>();
            pdfService.Setup(s => s.GeneratePdfFromHtml(It.IsAny<PdfParams>()))
                .ReturnsAsync(() =>
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", "some_invoice.pdf");
                    return Result.Ok(new PdfResult(path));
                });
            services.AddSingleton(pdfService.Object);

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
        public static readonly CompanyProfile CompanyProfile = new CompanyProfile { Company = new User(CvnEmail.Create("companyProfile@mail.com").Value), Agency = Agency };
        public static readonly Invoice Invoice = new Invoice
        {
            CompanyId = CompanyProfile.Id,
            InvoiceNumber = 1,
            NightShiftRate = 1,
            HolidayRate = 1,
            OverTimeRate = 1,
            VacationsRate = 1,
            HstRate = 1,
            BonusRate = 1,
            SubTotal = 1,
            Hst = 1,
            TotalNet = 1
        };

        public static void Seed(this CovenantContext context)
        {
            context.Add(CompanyProfile);
            context.AddRange(Invoice);
            context.SaveChanges();
        }
    }
}