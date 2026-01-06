using Covenant.Api;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Covenant.Integration.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseWebRoot("../../../../Covenant.Api/wwwroot")
                .CaptureStartupErrors(true)
                .UseStartup<TStartup>();
        }
    }

    public class CustomWebApplicationFactory : CustomWebApplicationFactory<Program>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseWebRoot("../../../../Covenant.Api/wwwroot")
                .UseEnvironment("Testing")
                .Configure(app =>
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
                });
        }
    }

    public static class IntegrationTestConfiguration
    {
        public static readonly bool UseInMemoryDatabase = true;
        public static readonly Func<string, string> ConnectionString = database =>
            $"Server=localhost;Database={database};UserId=postgres;Password=BDzJ9sH4U4EWu2aj;Port=5432";
    }
}