using Covenant.Api.AccountingModule.InvoiceDocument.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Pdf;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Accounting;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Globalization;
using Xunit;

namespace Covenant.Integration.Tests.AccountingModule.InvoiceDocument
{
    public class AccountingInvoiceDocumentV4ControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AccountingInvoiceDocumentV4ControllerTest.Startup>>
    {
        private readonly HttpClient _client;
        private static readonly string RouteName = AccountingInvoiceDocumentV4Controller.RouteName.Replace("{invoiceId}", Startup.FakeInvoice.Id.ToString());

        public AccountingInvoiceDocumentV4ControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Fact]
        public async Task GetPdf()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RouteName}/PDF");
            response.EnsureSuccessStatusCode();
            string expectedInvoiceHtml = Path.Combine(Directory.GetCurrentDirectory(), "Common", "fake_invoice_usa.html");
            FileAssert.Equal(expectedInvoiceHtml, Startup.InvoiceHtmlPath);
        }

        [Fact]
        public async Task Email()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync($"{RouteName}/Email", new { });
            response.EnsureSuccessStatusCode();
        }

        public class Startup
        {
            private static readonly DateTime FakeNow = new DateTime(2019, 01, 01);
            private static readonly City Miami = new City { Value = "Miami", Province = new Province { Code = "FL", Country = new Country { Code = "USA" } } };
            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency(
                "XYZ USA", "98765432109")
            {
                PhonePrincipalExt = 404
            };

            private static readonly CompanyProfile FakeCompany = CompanyProfile.AgencyCreateCompany(
                new User(CvnEmail.Create("c@c.us").Value),
                FakeAgency.Id,
                CompanyName.Create("ABC USA").Value,
                CompanyName.Create("ABC USA").Value,
                "6479087654",
                303,
                "6488087654",
                101,
                default,
                new CompanyProfileIndustry("Other"),
                null,
                null,
                null,
                default,
                "juan@covenantgroupl.com",
                CompanyStatus.Lead,
                null).Value;

            public static string InvoiceHtmlPath;

            public static InvoiceUSA FakeInvoice;

            public Startup()
            {
                FakeAgency.AddLocation(new Location { Address = "1701 Coral Way", PostalCode = "33145", City = Miami }, true);
                FakeCompany.AddLocation(Location.Create(Miami.Id, "2525 SW 3rd Ave", "M4P1M7").Value, true);
                FakeInvoice = InvoiceUSA.Create(1, FakeNow, FakeCompany.Id,
                        new[] { new InvoiceUSAItem(1, 1, "Regular") }, Array.Empty<InvoiceUSADiscount>(), new ProvinceTaxModel { Tax1 = 0.06m })
                    .Value;
                FakeInvoice.WeekEnding = FakeNow;
                FakeInvoice.BillFromAddress = FakeAgency.BillingAddress.FormattedAddress;
                FakeInvoice.BillFromPhone = FakeAgency.FormattedPhone;
                FakeInvoice.BillToAddress = FakeCompany.BillingAddress.FormattedAddress;
                FakeInvoice.BillToPhone = FakeCompany.FormattedPhone;
                FakeInvoice.BillToFax = FakeCompany.FormattedFax;
                FakeInvoice.BillToEmail = FakeCompany.Company.Email;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddAgencyPersonnelRole(FakeAgency.Id);
                });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(FakeNow);
                services.AddSingleton(timeService.Object);
                var payStubContainer = new Mock<IInvoicesContainer>();
                services.AddSingleton(payStubContainer.Object);
                var pdfGeneratorService = new Mock<IPdfGeneratorService>();
                pdfGeneratorService.Setup(s => s.GeneratePdfFromHtml(It.IsAny<PdfParams>()))
                    .Callback<PdfParams>(pdfParams =>
                    {
                        InvoiceHtmlPath = Path.Combine(Directory.GetCurrentDirectory(), "Common", "fake_invoice_usa.html");
                        File.WriteAllText(InvoiceHtmlPath, pdfParams.Html);
                    })
                    .ReturnsAsync(Result.Ok(new PdfResult(Path.Combine(Directory.GetCurrentDirectory(), "Common", "Fake_Invoice.pdf"))));
                services.AddSingleton(pdfGeneratorService.Object);
                services.AddSingleton<InvoicePdf>();
                services.AddSingleton<AgencyIdFilter>();
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
                context.City.Add(Miami);
                context.Agencies.Add(FakeAgency);
                context.CompanyProfile.Add(FakeCompany);
                context.InvoiceUSA.Add(FakeInvoice);
                context.SaveChanges();
            }
        }
    }
}