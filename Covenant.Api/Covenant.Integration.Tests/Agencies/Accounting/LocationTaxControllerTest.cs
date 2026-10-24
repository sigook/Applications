using Covenant.Api.Controllers.Agencies.Accounting;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Models.Locations;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Accounting;

public class LocationTaxControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<LocationTaxControllerTest.Startup, LocationTaxControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public LocationTaxControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _data = factory.Data;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, CovenantConstants.Role.Admin);
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private static string Uri(Location location) => $"{LocationTaxController.RouteName}/{location.Id}/tax";

    [Fact]
    public async Task Get_WithoutTax_ReturnsNoContent()
    {
        HttpResponseMessage response = await _client.GetAsync(Uri(_data.WithoutTax));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_StoresThePercentageAsAFraction_AndGetReturnsThePercentage()
    {
        (await _client.PutAsJsonAsync(Uri(_data.ForCreate), new LocationTaxModel { Tax1 = 13m })).EnsureSuccessStatusCode();

        Assert.Equal(0.13m, (await Context.LocationTaxes.AsNoTracking().SingleAsync(t => t.LocationId == _data.ForCreate.Id)).Tax1);
        var tax = await _client.GetFromJsonAsync<LocationTaxModel>(Uri(_data.ForCreate));
        Assert.Equal(13m, tax.Tax1);
    }

    [Fact]
    public async Task Put_Again_UpdatesTheSameRow()
    {
        (await _client.PutAsJsonAsync(Uri(_data.ForUpdate), new LocationTaxModel { Tax1 = 5m })).EnsureSuccessStatusCode();
        (await _client.PutAsJsonAsync(Uri(_data.ForUpdate), new LocationTaxModel { Tax1 = 15m })).EnsureSuccessStatusCode();

        var rows = await Context.LocationTaxes.AsNoTracking().Where(t => t.LocationId == _data.ForUpdate.Id).ToListAsync();
        Assert.Equal(0.15m, Assert.Single(rows).Tax1);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Put_OutOfRange_ReturnsBadRequest(decimal tax)
    {
        HttpResponseMessage response = await _client.PutAsJsonAsync(Uri(_data.ForInvalid), new LocationTaxModel { Tax1 = tax });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(await Context.LocationTaxes.AnyAsync(t => t.LocationId == _data.ForInvalid.Id));
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

    public class Data : ITestData
    {
        private readonly City city = FakeData.FakeCity();

        public Location WithoutTax { get; }
        public Location ForCreate { get; }
        public Location ForUpdate { get; }
        public Location ForInvalid { get; }

        public Data()
        {
            WithoutTax = FakeData.FakeLocation(city);
            ForCreate = FakeData.FakeLocation(city);
            ForUpdate = FakeData.FakeLocation(city);
            ForInvalid = FakeData.FakeLocation(city);
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(city);
            context.Locations.AddRange(WithoutTax, ForCreate, ForUpdate, ForInvalid);
            context.SaveChanges();
        }
    }
}
