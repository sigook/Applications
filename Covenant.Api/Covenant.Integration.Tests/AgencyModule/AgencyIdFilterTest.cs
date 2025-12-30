using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Sigook.Agency;
using Covenant.Api.Utils;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Interfaces;
using Covenant.Infrastructure.Context;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
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

        public class Startup
        {
            private static readonly User FakeUser = new User(CvnEmail.Create("a.a@sigook.com").Value);
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(FakeUser.Id);
                    o.AddAgencyPersonnelRole();
                });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton(new Mock<ITimeService>().Object);
                services.AddSingleton<AgencyIdFilter>();
                services.AddSingleton(new Mock<IDefaultLogoProvider>().Object);
                services.AddAutoMapper(Assembly.Load("Covenant.Api"));
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

                var agency = new Agency("agency", "3459876543");
                var agencyPersonnel = AgencyPersonnel.CreatePrimary(agency.Id, FakeUser);
                context.Agencies.Add(agency);
                context.AgencyPersonnel.Add(agencyPersonnel);
                context.SaveChanges();
            }
        }
    }
}