using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Profile;
using Covenant.Api.Controllers.Agencies.Sales;
using Covenant.Api.Controllers.Agencies;
using Covenant.Api.Utils;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agencies;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace Covenant.Integration.Tests.Agencies;

public class AgencyIdFilterTest(CustomWebApplicationFactory<AgencyIdFilterTest.Startup> factory) : ControllerBase, IClassFixture<CustomWebApplicationFactory<AgencyIdFilterTest.Startup>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(AgenciesController.RouteName);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetProfile_WithoutQuery_ReturnsFirstTokenAgency()
    {
        HttpResponseMessage response = await _client.GetAsync(AgencyProfileController.RouteName);
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<AgencyModel>();
        Assert.Equal(Startup.FakeAgency.Id, model.Id);
    }

    [Fact]
    public async Task GetProfile_WithAccessibleAgencyIdQuery_ReturnsThatAgency()
    {
        var model = await _client.GetFromJsonAsync<AgencyModel>($"{AgencyProfileController.RouteName}?agencyId={Startup.SecondAgency.Id}");
        Assert.Equal(Startup.SecondAgency.Id, model.Id);
    }

    [Fact]
    public async Task GetProfile_WithForeignAgencyIdQuery_ReturnsFirstTokenAgency()
    {
        var model = await _client.GetFromJsonAsync<AgencyModel>($"{AgencyProfileController.RouteName}?agencyId={Guid.NewGuid()}");
        Assert.Equal(Startup.FakeAgency.Id, model.Id);
    }

    [Fact]
    public async Task GetSubAgencies_WithoutQuery_UsesEveryAccessibleAgency()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<AgencyModel>>(AgenciesController.RouteName);
        Assert.Contains(page.Items, a => a.Id == Startup.ChildAgency.Id);
    }

    [Fact]
    public async Task GetSubAgencies_WithAgencyIdsQuery_UsesOnlySelectedAgencies()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<AgencyModel>>($"{AgenciesController.RouteName}?agencyIds={Startup.FakeAgency.Id}");
        Assert.DoesNotContain(page.Items, a => a.Id == Startup.ChildAgency.Id);
    }

    [Fact]
    public async Task GetAccessibleAgencies_ReturnsTheAgenciesOfTheClaims()
    {
        var agencies = await _client.GetFromJsonAsync<List<AgencyOptionModel>>($"{AgencyProfileController.RouteName}/agencies");
        Assert.Equal(new[] { Startup.FakeAgency.Id, Startup.SecondAgency.Id }.Order(), agencies.Select(a => a.Id).Order());
    }

    public class Startup
    {
        private static readonly User FakeUser = new User(CvnEmail.Create("a.a@sigook.com").Value);
        public static readonly Agency FakeAgency = new Agency("agency", "3459876543") { User = FakeData.FakeUser() };
        public static readonly Agency SecondAgency = new Agency("second", "3459876544") { User = FakeData.FakeUser() };
        public static readonly Agency ChildAgency = new Agency("child", "3459876545") { User = FakeData.FakeUser(), AgencyParentId = SecondAgency.Id };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o =>
            {
                o.AddSub(FakeUser.Id);
                o.AddAgencyPersonnelRole(FakeAgency.Id);
                o.Identity.AddClaim(new Claim(CovenantConstants.AgencyId, SecondAgency.Id.ToString()));
            });
            services.AddTestDatabase();
            services.AddSingleton<TimeProvider>(new FakeTimeProvider());
            services.AddSingleton<AgencyIdFilter>();
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
            context.Agencies.AddRange(FakeAgency, SecondAgency, ChildAgency);
            context.Users.Add(FakeUser);
            context.SaveChanges();
        }
    }
}
