using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Sigook.Agency;
using Covenant.Api.Utils;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Agency;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http.Json;
using System.Reflection;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule
{
    public class AgencyIdFilterTest : ControllerBase, IClassFixture<CustomWebApplicationFactory<AgencyIdFilterTest.Startup>>
    {
        private readonly HttpClient _client;

        public AgencyIdFilterTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(AgencyController.RouteName);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetProfile_IgnoresAgencyIdClaimFromToken()
        {
            HttpResponseMessage response = await _client.GetAsync($"{AgencyController.RouteName}/Profile");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<AgencyModel>();
            Assert.Equal(Startup.FakeAgency.Id, model.Id);
        }

        public class Startup
        {
            private static readonly User FakeUser = new User(CvnEmail.Create("a.a@sigook.com").Value);
            public static readonly Agency FakeAgency = new Agency("agency", "3459876543") { User = FakeData.FakeUser() };

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(FakeUser.Id);
                    o.AddAgencyPersonnelRole(Guid.NewGuid());
                });
                services.AddTestDatabase();
                services.AddSingleton(new Mock<ITimeService>().Object);
                services.AddSingleton<AgencyIdFilter>();
                services.AddSingleton(new Mock<IDefaultLogoProvider>().Object);
            }

            public void Configure(IApplicationBuilder app, CovenantContext context)
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

                var agencyPersonnel = AgencyPersonnel.CreatePrimary(FakeAgency.Id, FakeUser);
                context.Agencies.Add(FakeAgency);
                context.AgencyPersonnel.Add(agencyPersonnel);
                context.SaveChanges();
            }
        }
    }
}