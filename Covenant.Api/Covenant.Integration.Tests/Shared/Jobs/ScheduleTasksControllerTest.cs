using Covenant.Api.Controllers.Shared.Jobs;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
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
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Shared.Jobs;

public class ScheduleTasksControllerTest(CustomWebApplicationFactory<ScheduleTasksControllerTest.Startup> factory) : IClassFixture<CustomWebApplicationFactory<ScheduleTasksControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory = factory;

    [Theory]
    [InlineData("NotificationSinExpiration")]
    [InlineData("WarnLicensesExpiration")]
    [InlineData("MarkOverdueInvoices")]
    public async Task Execute(string action)
    {
        using HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, Startup.FunctionsClientId);

        var response = await client.PostAsync($"{ScheduleTasksController.RouteName}/{action}", new StringContent(""));

        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("NotificationSinExpiration")]
    [InlineData("WarnLicensesExpiration")]
    [InlineData("MarkOverdueInvoices")]
    public async Task CallerThatIsNotTheFunctionsClient_ReturnsForbidden(string action)
    {
        using HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, CovenantConstants.Role.Worker);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, Guid.NewGuid().ToString());

        var response = await client.PostAsync($"{ScheduleTasksController.RouteName}/{action}", new StringContent(""));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public class Startup
    {
        public const string FunctionsClientId = "sigook.functions.test";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.Configure<IdentityConfiguration>(o => o.FunctionsClientId = FunctionsClientId);
            services.AddTestAuthenticationBuilder().AddTestAuth(o => { });
            services.AddSingleton(new Mock<IGeocodeService>().Object);
            services.AddSingleton(Mock.Of<IPayStubsContainer>());
            services.AddSingleton(Mock.Of<ITeamsService>());
            services.AddSingleton(Mock.Of<ICurrentUserService>());
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
