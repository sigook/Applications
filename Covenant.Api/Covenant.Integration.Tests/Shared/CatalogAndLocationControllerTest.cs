using Covenant.Api.Controllers.Shared;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Shared;

public class CatalogAndLocationControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<CatalogAndLocationControllerTest.Startup, CatalogAndLocationControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;

    public CatalogAndLocationControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _data = factory.Data;
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private HttpClient As(string role)
    {
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
        return client;
    }

    private HttpClient Anyone() => _factory.CreateClient();

    [Theory]
    [InlineData("availability", "Full time")]
    [InlineData("day", "Monday")]
    [InlineData("gender", "Female")]
    [InlineData("language", "French")]
    [InlineData("industry", "Manufacturing")]
    public async Task Catalogs_ReturnTheSeededValues(string catalog, string expected)
    {
        var values = await Anyone().GetFromJsonAsync<List<BaseModel<Guid>>>($"{CatalogController.RouteName}/{catalog}");

        Assert.Contains(values, v => v.Value == expected);
    }

    [Theory]
    [InlineData("skills")]
    [InlineData("companyStatus")]
    [InlineData("tax-categories")]
    public async Task StaticCatalogs_AreNotEmpty(string catalog)
    {
        HttpResponseMessage response = await Anyone().GetAsync($"{CatalogController.RouteName}/{catalog}");

        response.EnsureSuccessStatusCode();
        Assert.NotEqual("[]", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task AddIndustry_AsAgency_CreatesIt()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Recruiting)
            .PostAsJsonAsync($"{CatalogController.RouteName}/industry", new BaseModel<Guid> { Value = "Aerospace" });

        response.EnsureSuccessStatusCode();
        Assert.True(await Context.Industries.AnyAsync(i => i.Value == "Aerospace"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddIndustry_Blank_ReturnsBadRequest(string value)
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Recruiting)
            .PostAsJsonAsync($"{CatalogController.RouteName}/industry", new BaseModel<Guid> { Value = value });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddIndustry_AsWorker_IsForbidden()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Worker)
            .PostAsJsonAsync($"{CatalogController.RouteName}/industry", new BaseModel<Guid> { Value = "Forbidden Industry" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Countries_IncludeCanadaAndTheUsa()
    {
        var countries = await Anyone().GetFromJsonAsync<List<CountryModel>>($"{LocationController.RouteName}/country");

        Assert.Contains(countries, c => c.Id == _data.Canada.Id);
        Assert.Contains(countries, c => c.Id == _data.Usa.Id);
    }

    [Fact]
    public async Task Provinces_ByCountryIdAndByCode_ReturnTheSameProvinces()
    {
        var byId = await Anyone().GetFromJsonAsync<List<ProvinceModel>>($"{LocationController.RouteName}/province/{_data.Canada.Id}");
        var byCode = await Anyone().GetFromJsonAsync<List<ProvinceModel>>($"{LocationController.RouteName}/province/ca");

        Assert.Contains(byId, p => p.Id == _data.Ontario.Id);
        Assert.Equal(byId.Select(p => p.Id).Order(), byCode.Select(p => p.Id).Order());
    }

    [Fact]
    public async Task Provinces_UnsupportedCountryCode_ReturnsUnprocessableEntity()
    {
        HttpResponseMessage response = await Anyone().GetAsync($"{LocationController.RouteName}/province/MX");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Cities_CollapseDuplicatedNames()
    {
        var cities = await Anyone().GetFromJsonAsync<List<CityModel>>($"{LocationController.RouteName}/city/{_data.Ontario.Id}");

        Assert.Single(cities, c => c.Value == "TORONTO");
        Assert.Contains(cities, c => c.Value == "OTTAWA");
    }

    [Fact]
    public async Task AddCity_StoresTheNameInUpperCase()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Recruiting).PostAsJsonAsync($"{LocationController.RouteName}/city",
            new CityModel { Value = "Kingston", Province = new ProvinceModel { Id = _data.Ontario.Id } });

        response.EnsureSuccessStatusCode();
        Assert.True(await Context.Cities.AnyAsync(c => c.Value == "KINGSTON" && c.ProvinceId == _data.Ontario.Id));
    }

    [Fact]
    public async Task AddCity_WithoutName_ReturnsBadRequest()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Recruiting).PostAsJsonAsync($"{LocationController.RouteName}/city",
            new CityModel { Value = "", Province = new ProvinceModel { Id = _data.Ontario.Id } });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProvinceSettings_AsAdmin_AreUpserted()
    {
        HttpClient admin = As(CovenantConstants.Role.Admin);
        string uri = $"{LocationController.RouteName}/province/{_data.Quebec.Id}/settings";

        (await admin.PutAsJsonAsync(uri, new ProvinceSettingsModel { PaidHolidays = true, OvertimeStartsAfter = 40 })).EnsureSuccessStatusCode();
        (await admin.PutAsJsonAsync(uri, new ProvinceSettingsModel { PaidHolidays = false, OvertimeStartsAfter = 44 })).EnsureSuccessStatusCode();

        ProvinceSetting settings = await Context.ProvinceSettings.AsNoTracking().SingleAsync(s => s.ProvinceId == _data.Quebec.Id);
        Assert.False(settings.PaidHolidays);
        Assert.Equal(TimeSpan.FromHours(44), settings.OvertimeStartsAfter);
    }

    [Fact]
    public async Task ProvinceSettings_AsRecruiting_IsForbidden()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Recruiting)
            .PutAsJsonAsync($"{LocationController.RouteName}/province/{_data.Quebec.Id}/settings", new ProvinceSettingsModel { PaidHolidays = true });

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
            app.UseResponseCaching();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    public class Data : ITestData
    {
        public Country Canada { get; } = FakeData.FakeCountry("CA");
        public Country Usa { get; } = FakeData.FakeCountry("USA");
        public Province Ontario { get; }
        public Province Quebec { get; }

        public Data()
        {
            Ontario = FakeData.FakeProvince(Canada);
            Quebec = FakeData.FakeProvince(Canada);
        }

        public void Seed(CovenantContext context)
        {
            context.Countries.AddRange(Canada, Usa);
            context.Provinces.AddRange(Ontario, Quebec);
            context.Cities.AddRange(new City("TORONTO", Ontario.Id), new City("TORONTO", Ontario.Id), new City("OTTAWA", Ontario.Id));
            context.Availabilities.Add(new Availability("Full time"));
            context.Days.Add(new Day("Monday"));
            context.Genders.Add(new Gender("Female"));
            context.Languages.Add(new Language("French"));
            context.Industries.Add(Industry.Create("Manufacturing").Value);
            context.SaveChanges();
        }
    }
}
