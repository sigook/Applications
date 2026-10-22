using Covenant.Api.Controllers.Agency.Profile;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Models.Agency;
using Covenant.Common.Interfaces;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agency.Profile;

public class PersonnelAgenciesControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<PersonnelAgenciesControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public PersonnelAgenciesControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string RequestUri() => PersonnelAgenciesController.RouteName;

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<IEnumerable<PersonnelAgencyModel>>();
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

        var context = _factory.Services.GetRequiredService<CovenantContext>();
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
            services.AddTestDatabase();
            services.AddSingleton<AgencyIdFilter>();
        }

        public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency1 = new Covenant.Common.Entities.Agency.Agency(default, default) { User = FakeData.FakeUser() };

        public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency2 = new Covenant.Common.Entities.Agency.Agency(default, default) { User = FakeData.FakeUser() };

        public static readonly User FakePersonnel = new User(CvnEmail.Create("recruiter@sigook.com").Value);

        public static readonly AgencyPersonnel FakeAgencyPersonnel1 = AgencyPersonnel.CreatePrimary(FakeAgency1.Id, FakePersonnel, "A1 Recruiter");

        public static readonly AgencyPersonnel FakeAgencyPersonnel2 = AgencyPersonnel.Create(FakeAgency2.Id, FakePersonnel, false, "A2 Recruiter");

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
