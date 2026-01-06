using Covenant.Api.AgencyModule.AgencyPersonnel.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Models.Agency;
using Covenant.Common.Interfaces;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyPersonnelTest
{
    public class PersonnelAgencyControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<PersonnelAgencyControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public PersonnelAgencyControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => PersonnelAgencyController.RouteName;

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<PersonnelAgencyModel>>();
            AgencyPersonnel entity = Startup.FakeAgencyPersonnel1;
            var model = list.Single(c => c.Id == entity.Id);
            Assert.Equal(entity.Agency.FullName, model.Name);
            Assert.Equal(entity.Agency.User.Email, model.Email);
            Assert.True(model.IsPrimary);
        }

        [Fact]
        public async Task Put()
        {
            Guid id = Startup.FakeAgencyPersonnel2.Id;
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}", new { });
            response.EnsureSuccessStatusCode();

            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.True((await context.AgencyPersonnel.SingleAsync(s => s.Id == id)).IsPrimary);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(FakePersonnel.Id);
                        o.AddAgencyPersonnelRole();
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly Agency FakeAgency1 = new Agency(default, default);

            public static readonly Agency FakeAgency2 = new Agency(default, default);

            public static readonly User FakePersonnel = new User(CvnEmail.Create("recruiter@sigook.com").Value);

            public static readonly AgencyPersonnel FakeAgencyPersonnel1 = AgencyPersonnel.CreatePrimary(FakeAgency1.Id, FakePersonnel, "A1 Recruiter");

            public static readonly AgencyPersonnel FakeAgencyPersonnel2 = AgencyPersonnel.Create(FakeAgency1.Id, FakePersonnel, false, "A2 Recruiter");

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
                FakeAgency1.User = new User(CvnEmail.Create("c@mail.com").Value);
                FakeAgency2.User = new User(CvnEmail.Create("c2@mail.com").Value);
                context.Agencies.AddRange(FakeAgency1, FakeAgency2);
                context.AgencyPersonnel.AddRange(FakeAgencyPersonnel1, FakeAgencyPersonnel2);
                context.SaveChanges();
            }
        }
    }
}