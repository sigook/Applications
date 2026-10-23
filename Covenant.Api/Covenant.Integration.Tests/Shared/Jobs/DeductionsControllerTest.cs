using Covenant.Api.Controllers.Shared.Jobs;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Shared.Jobs;

public class DeductionsControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<DeductionsControllerTest.Startup>>
{
    private const string CppBlob = "CPP WEEKLY 2026.pdf";
    private const string TaxBlob = "TAX MONTHLY 2026.pdf";

    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public DeductionsControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private static ImportCraTableFromBlobModel Model(string blobName, PayPeriod payPeriod, int year = 2026) =>
        new() { BlobName = blobName, PayPeriod = payPeriod, Year = year };

    private async Task<int> Import(string action, ImportCraTableFromBlobModel model)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{DeductionsController.RouteName}/{action}", model);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    [Fact]
    public async Task CppBlob_StoresTheWeeklyBrackets_AndReimportReplacesThem()
    {
        int first = await Import("Cpp/Blob", Model(CppBlob, PayPeriod.Weekly));
        int second = await Import("Cpp/Blob", Model(CppBlob, PayPeriod.Weekly));

        Assert.True(first >= 1000);
        Assert.Equal(first, second);
        Assert.Equal(first, await Context.CppDeductions.CountAsync(c => c.Year == 2026 && c.PayPeriod == PayPeriod.Weekly));
    }

    [Fact]
    public async Task TaxBlob_StoresFederalAndProvincialBrackets()
    {
        int imported = await Import("Tax/Blob", Model(TaxBlob, PayPeriod.Monthly));

        Assert.True(imported >= 200);
        var rows = Context.TaxDeductions.Where(t => t.Year == 2026 && t.PayPeriod == PayPeriod.Monthly);
        Assert.Equal(imported, await rows.CountAsync());
        Assert.True(await rows.AnyAsync(t => t.TaxType == TaxType.Federal));
        Assert.True(await rows.AnyAsync(t => t.TaxType == TaxType.Provincial));
    }

    [Fact]
    public async Task MissingBlob_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{DeductionsController.RouteName}/Cpp/Blob", Model("CPP MONTHLY 2026.pdf", PayPeriod.Monthly));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(await Context.CppDeductions.AnyAsync(c => c.Year == 2026 && c.PayPeriod == PayPeriod.Monthly));
    }

    [Fact]
    public async Task NonPdfBlob_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{DeductionsController.RouteName}/Tax/Blob", Model("TAX MONTHLY 2026.xlsx", PayPeriod.Monthly));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TaxTableSentToTheCppEndpoint_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{DeductionsController.RouteName}/Cpp/Blob", Model(TaxBlob, PayPeriod.BiWeekly));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(await Context.CppDeductions.AnyAsync(c => c.Year == 2026 && c.PayPeriod == PayPeriod.BiWeekly));
    }

    public class Startup
    {
        private static readonly string CraFolder = Path.Combine(AppContext.BaseDirectory, "Files", "Cra");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
            services.AddTestDatabase();
            var container = new Mock<ICraTablesContainer>();
            container.Setup(c => c.Download(CppBlob)).ReturnsAsync(File.ReadAllBytes(Path.Combine(CraFolder, "cpp_weekly_2026.pdf")));
            container.Setup(c => c.Download(TaxBlob)).ReturnsAsync(File.ReadAllBytes(Path.Combine(CraFolder, "tax_monthly_2026.pdf")));
            services.AddSingleton(container.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
