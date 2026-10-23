using Covenant.Api.Controllers.Agencies;
using Covenant.Common.Constants;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies;

public class NotificationsControllerTest(CustomWebApplicationFactory<NotificationsControllerTest.Startup> factory)
    : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<NotificationsControllerTest.Startup>>
{
    private HttpClient As(string role)
    {
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
        return client;
    }

    [Fact]
    public async Task AgencyStaff_GetsTheBellPayload()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Recruiting).GetAsync(NotificationsController.RouteName);

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Worker_IsForbidden()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Worker).GetAsync(NotificationsController.RouteName);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
            services.AddTestDatabase();
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
