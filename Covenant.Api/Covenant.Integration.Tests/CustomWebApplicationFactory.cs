using Covenant.Api;
using Covenant.Integration.Tests.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Covenant.Integration.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Lazy<string> database = new(PostgresTestDatabase.CreateDatabase,
            LazyThreadSafetyMode.ExecutionAndPublication);

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .CaptureStartupErrors(false)
                    .UseStartup<TStartup>());
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            PostgresTestDatabase.Use(database.Value);
            return base.CreateHost(builder);
        }
    }

    public class CustomWebApplicationFactory : CustomWebApplicationFactory<Program>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => webBuilder
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
                    }));
        }
    }
}
