using Covenant.Api.Controllers.Shared.Jobs;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces.Agencies;
using Covenant.Core.BL.Services.Agencies;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Requests;
using Covenant.Infrastructure.Repositories.Workers;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.Shared.Jobs;

public class ScheduleTasksControllerTest : IClassFixture<CustomWebApplicationFactory<ScheduleTasksControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public ScheduleTasksControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    [Theory]
    [InlineData("NotificationSinExpiration")]
    [InlineData("WarnLicensesExpiration")]
    public async Task Execute(string action)
    {
        var response = await _client.PostAsync($"{ScheduleTasksController.RouteName}/{action}", new StringContent(""));
        response.EnsureSuccessStatusCode();
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => { });
            services.AddSingleton(new Mock<IGeocodeService>().Object);
            services.AddTestDatabase();
            services.AddSingleton<IAgencyService, AgencyService>();
            services.AddSingleton<ITimesheetRepository, TimesheetRepository>();
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<ICatalogRepository, CatalogRepository>();
            services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton<IWorkerRepository, WorkerRepository>();
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
