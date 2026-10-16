using System.Net.Http.Json;
using Covenant.Api.Controllers.Shared;
using Covenant.Common.Configuration;
using Covenant.Common.Models;
using Covenant.Integration.Tests.Configuration;
using Xunit;

namespace Covenant.Integration.Tests.Shared;

public class MobileAppControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<MobileAppControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public MobileAppControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetVersionReturnsConfiguredValues()
    {
        var response = await _client.GetAsync($"{MobileAppController.RouteName}/version");
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<MobileAppVersionModel>();
        Assert.NotNull(model);
        Assert.Equal("2026.10.9", model.MinimumVersion);
        Assert.Equal("https://play.google.com/store/apps/details?id=com.sigook.sigook", model.AndroidStoreUrl);
        Assert.Equal("https://apps.apple.com/ca/app/id1446736193", model.IosStoreUrl);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestDatabase();
            services.Configure<MobileAppConfiguration>(o =>
            {
                o.MinimumVersion = "2026.10.9";
                o.AndroidStoreUrl = "https://play.google.com/store/apps/details?id=com.sigook.sigook";
                o.IosStoreUrl = "https://apps.apple.com/ca/app/id1446736193";
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
