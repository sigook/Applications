using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Companies.Profile;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Locations;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Companies;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Companies.Profile;

public class LocationsControllerTest : ControllerBase, IClassFixture<CustomWebApplicationFactory<LocationsControllerTest.Startup>>
{
    private readonly HttpClient _client;
    public LocationsControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(LocationsController.RouteName);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<LocationDetailModel>>();
        Assert.NotEmpty(list);
        ICollection<CompanyProfileLocation> locations = Data.CompanyProfile.Locations;
        foreach (CompanyProfileLocation item in locations)
        {
            Assert.Contains(list, m => m.City.Id == item.Location.City.Id);
            Assert.Contains(list, m => m.City.Province.Id == item.Location.City.Province.Id);
            Assert.Contains(list, m => m.City.Province.Country.Id == item.Location.City.Province.Country.Id);
            Assert.Contains(list, m => m.Address == item.Location.Address);
            Assert.Contains(list, m => m.PostalCode == item.Location.PostalCode);
        }
    }

    private static CompanyProfileLocationDetailModel NewLocation(string address, bool isBilling = false) => new()
    {
        Address = address,
        PostalCode = "M5H1A1",
        City = new CityModel { Id = Data.CompanyProfile.Locations.First().Location.City.Id },
        IsBilling = isBilling,
        Latitude = 43.65,
        Longitude = -79.38
    };

    private Task<List<LocationDetailModel>> List() => _client.GetFromJsonAsync<List<LocationDetailModel>>(LocationsController.RouteName);

    [Fact]
    public async Task Post_AddsALocationToTheCallerCompany()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(LocationsController.RouteName, NewLocation("1 New Street", isBilling: true));

        response.EnsureSuccessStatusCode();
        LocationDetailModel created = Assert.Single(await List(), l => l.Address == "1 New Street");
        Assert.True(created.IsBilling);
    }

    [Fact]
    public async Task Post_WithoutCity_ReturnsBadRequest()
    {
        CompanyProfileLocationDetailModel model = NewLocation("No City Street");
        model.City = null;

        HttpResponseMessage response = await _client.PostAsJsonAsync(LocationsController.RouteName, model);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesAddressAndBillingFlag()
    {
        (await _client.PostAsJsonAsync(LocationsController.RouteName, NewLocation("2 Before Street"))).EnsureSuccessStatusCode();
        Guid id = (await List()).Single(l => l.Address == "2 Before Street").Id;

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{LocationsController.RouteName}/{id}", NewLocation("2 After Street", isBilling: true));

        response.EnsureSuccessStatusCode();
        LocationDetailModel updated = Assert.Single(await List(), l => l.Id == id);
        Assert.Equal("2 After Street", updated.Address);
        Assert.True(updated.IsBilling);
    }

    [Fact]
    public async Task Put_UnknownLocation_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{LocationsController.RouteName}/{Guid.NewGuid()}", NewLocation("Nowhere"));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesTheLocation()
    {
        (await _client.PostAsJsonAsync(LocationsController.RouteName, NewLocation("3 Delete Street"))).EnsureSuccessStatusCode();
        Guid id = (await List()).Single(l => l.Address == "3 Delete Street").Id;

        (await _client.DeleteAsync($"{LocationsController.RouteName}/{id}")).EnsureSuccessStatusCode();

        Assert.DoesNotContain(await List(), l => l.Id == id);
    }

    [Fact]
    public async Task Delete_UnknownLocation_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{LocationsController.RouteName}/{Guid.NewGuid()}");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddSub(Data.CompanyProfile.Company.Id);
                    o.AddCompanyRole();
                });
            services.AddTestDatabase();
            services.AddSingleton<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<CompanyIdFilter>();
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
            context.CompanyProfiles.Add(Data.CompanyProfile);
            context.SaveChanges();
        }
    }

    private static class Data
    {
        public static readonly CompanyProfile CompanyProfile = new CompanyProfile
        {
            Company = new User(CvnEmail.Create("company@company.com").Value),
            Locations = new List<CompanyProfileLocation>
            {
                new CompanyProfileLocation {Location = new Location {Address = "ABC",City = new City {Province = new Province{Country = FakeData.FakeCountry("CA")}}}},
                new CompanyProfileLocation {Location = new Location {Address = "DFG",City = new City {Province = new Province{Country =FakeData.FakeCountry("CA")}}}}
            }
        , Industry = new CompanyProfileIndustry("Test") , Agency = FakeData.FakeAgency() };
    }
}
