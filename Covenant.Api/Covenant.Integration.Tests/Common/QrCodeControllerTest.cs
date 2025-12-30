using Covenant.Api.Controllers.Sigook;
using Covenant.Infrastructure.Context;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.Common
{
    public class QrCodeControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<QrCodeControllerTest.Startup>>
    {
        private readonly HttpClient _client;

        public QrCodeControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(b => b.ConfigureAppConfiguration(((context, config) =>
                    config.AddJsonFile(Path.Combine(FileControllerTest.PathTestFiles, "appsettings.test.json"), false))))
                .CreateClient();
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync($"{QrCodeController.RouteName}/{Guid.NewGuid()}");
            response.EnsureSuccessStatusCode();
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o => { });
                services.AddDbContext<CovenantContext>(b =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            }

            public void Configure(IApplicationBuilder app)
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
            }
        }
    }
}