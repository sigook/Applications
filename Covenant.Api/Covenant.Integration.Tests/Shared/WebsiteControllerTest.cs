using System.Net.Http.Json;
using Covenant.Api.Controllers.Shared;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Website;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.Shared;

public class WebsiteControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WebsiteControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public WebsiteControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetJobsReturnsEmploymentTypeAsDisplayName()
    {
        var response = await _client.GetAsync($"{WebsiteController.RouteName}/jobs?countries=CA");
        response.EnsureSuccessStatusCode();
        var jobs = await response.Content.ReadFromJsonAsync<List<JobViewModel>>();
        var job = Assert.Single(jobs, j => j.RequestId == Data.FakeRequest.Id);
        Assert.Equal("Part Time", job.Type);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestDatabase();
            services.AddMemoryCache();
            services.AddSingleton(Mock.Of<IRazorViewToStringRenderer>());
        }

        public void Configure(IApplicationBuilder app, CovenantContext context)
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
            Data.Seed(context);
        }
    }

    private static class Data
    {
        public static readonly Request FakeRequest = FakeData.FakeRequest(startAt: new DateTime(2019, 01, 01));

        public static void Seed(CovenantContext context)
        {
            FakeRequest.CompanyProfile.Active = true;
            FakeRequest.EmploymentType = EmploymentType.PartTime;
            context.Requests.Add(FakeRequest);
            context.SaveChanges();
        }
    }
}
