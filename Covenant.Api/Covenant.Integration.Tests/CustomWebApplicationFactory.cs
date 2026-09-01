using Covenant.Api;
using Covenant.Integration.Tests.Configuration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Covenant.Integration.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Lazy<string> database = new(PostgresTestDatabase.CreateDatabase,
            LazyThreadSafetyMode.ExecutionAndPublication);

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .CaptureStartupErrors(false)
                .UseStartup<TStartup>();
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            PostgresTestDatabase.Use(database.Value);
            return base.CreateServer(builder);
        }
    }

    public class CustomWebApplicationFactory : CustomWebApplicationFactory<Program>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
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
}