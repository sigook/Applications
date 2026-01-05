using Covenant.Api.AccountingModule.PayStubDocument.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Pdf;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Worker;
using Covenant.Deductions.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Accounting;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.PayStubs.Services;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.Shared.PayStubDocument;

public class TestPayrollDocumentController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    public TestPayrollDocumentController(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetPdf()
    {
        HttpResponseMessage response = await _client.GetAsync($"api/V4/Accounting/PayStub/{Data.FakePayStub.Id}/Document/PDF");
        response.EnsureSuccessStatusCode();
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDefaultTestConfiguration();
        services.AddTestAuthenticationBuilder().AddTestAuth(o => { });
        services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
        services.AddSingleton<IPayStubRepository, PayStubRepository>();
        services.AddSingleton(new Mock<IPayStubsContainer>().Object);
        var pdfService = new Mock<IPdfGeneratorService>();
        pdfService.Setup(s => s.GeneratePdfFromHtml(It.IsAny<PdfParams>()))
            .ReturnsAsync(() =>
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", "some_pay_stub.pdf");
                return Result.Ok(new PdfResult(path));
            });
        services.AddSingleton(pdfService.Object);
        services.AddSingleton<PayStubPdf>();
        services.AddSingleton<AgencyIdFilter>();
        services.AddSingleton<ITimeService, TimeService>();
        services.AddSingleton(Rates.DefaultRates);
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
    public static readonly DateTime Now = new DateTime(2019, 01, 10);
    public static readonly Covenant.Common.Entities.Agency.Agency Agency = new Covenant.Common.Entities.Agency.Agency();
    public static readonly WorkerProfile FakeWorkerProfile = new WorkerProfile(new User(CvnEmail.Create("fake_worker_profile@mail.com").Value)) { Agency = Agency };
    public static PayStub FakePayStub;

    public static void Seed(this CovenantContext context)
    {
        var mockWorkerRepository = new Mock<IWorkerRepository>(); Mock.Of<IWorkerRepository>();
        mockWorkerRepository.Setup(r => r.GetWorkerProfileTaxCategory(It.IsAny<Guid>())).ReturnsAsync(new WorkerProfileTaxCategory
        {
            WorkerProfileId = FakeWorkerProfile.Id,
            FederalCategory = TaxCategory.Cc1,
            ProvincialCategory = TaxCategory.Cc1
        });
        FakePayStub = PayStubBuilder.PayStub(Rates.DefaultRates, new Mock<IPayrollDeductionsAndContributionsCalculator>().Object, mockWorkerRepository.Object)
            .WithPayStubNumber(1)
            .WithWorkerProfileId(FakeWorkerProfile.Id)
            .WithTypeOfWork("General Labour")
            .WithWorkBeginning(Now)
            .WithWorkEnding(Now.AddDays(1))
            .WithCreationDate(Now)
            .WithItems(new[]
            {
                PayStubItem.CreateItem("Regular",1,10,PayStubItemType.Regular).Value
            })
            .WithWageDetails(Array.Empty<PayStubWageDetail>())
            .WithPublicHolidaysToPay(Array.Empty<PayStubPublicHoliday>())
            .WithNoMoreDeductions()
            .WithoutReimbursement()
            .PayVacations()
            .Build().Result.Value;
        context.WorkerProfile.Add(FakeWorkerProfile);
        context.PayStub.Add(FakePayStub);
        context.SaveChanges();
    }
}