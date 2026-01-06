using Covenant.Api;
using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Sigook.Agency;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Location;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyLocation
{
    public class AgencyLocationControllerTest
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Province Ontario;
        private readonly City Toronto;
        private readonly City York;
        private readonly City Brampton;
        private readonly Covenant.Common.Entities.Agency.Agency FakeAgency;
        private readonly Covenant.Common.Entities.Agency.AgencyLocation FakeLocation;
        private readonly Covenant.Common.Entities.Agency.AgencyLocation FakeUpdateLocation;
        private readonly Covenant.Common.Entities.Agency.AgencyLocation FakeDeleteLocation;

        public AgencyLocationControllerTest()
        {
            _factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddDefaultTestConfiguration();
                        services.AddTestAuthenticationBuilder()
                            .AddTestAuth(o =>
                            {
                                o.AddAgencyPersonnelRole(FakeAgency.Id);
                            });
                        services.AddDbContext<CovenantContext>(b
                            => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                        services.AddSingleton<ITimeService, TimeService>();
                        services.AddSingleton<AgencyIdFilter>();
                    });
                });
            Ontario = new Province { Value = "Ontario", Country = new Country { Value = "Canada" } };
            Toronto = new City { Value = "Toronto", Province = Ontario };
            York = new City { Value = "York", Province = Ontario };
            Brampton = new City { Value = "Brampton", Province = Ontario };
            FakeAgency = new Covenant.Common.Entities.Agency.Agency { User = new User(CvnEmail.Create("agency@agency.com").Value) };
            FakeLocation = new Covenant.Common.Entities.Agency.AgencyLocation(Location.Create(York.Id, "567 Dundas", "A2A2A2").Value, FakeAgency.Id, true);
            FakeUpdateLocation = new Covenant.Common.Entities.Agency.AgencyLocation(Location.Create(York.Id, "543 Dundas update", "B3B3B3").Value, FakeAgency.Id);
            FakeDeleteLocation = new Covenant.Common.Entities.Agency.AgencyLocation(Location.Create(York.Id, "345 Dundas Delete", "C3C3C3").Value, FakeAgency.Id);
            _client = _factory.CreateClient();
            Seed(_factory.Services.GetService<CovenantContext>());
        }

        private string RequestUri() => AgencyLocationController.RouteName;

        [Fact]
        public async Task Post()
        {
            var model = new LocationModel
            {
                Address = "419 Bloor Ave",
                PostalCode = "12345",
                City = new CityModel(Toronto.Id, Toronto.Value),
                IsBilling = true,
            };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<LocationDetailModel>();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            Covenant.Common.Entities.Agency.AgencyLocation entity = await context.Set<Covenant.Common.Entities.Agency.AgencyLocation>().SingleAsync(c => c.LocationId == detail.Id);
            Assert.Equal(model.Address, entity.Location.Address);
            Assert.Equal(model.PostalCode, entity.Location.PostalCode);
            Assert.Equal(model.City.Id, entity.Location.City.Id);
            Assert.Equal(model.IsBilling, entity.IsBilling);
        }

        [Fact]
        public async Task Put()
        {
            var model = new LocationModel
            {
                Address = "654 Bloor Update",
                PostalCode = "12345",
                City = new CityModel(Brampton.Id, Brampton.Value),
                IsBilling = false
            };
            Guid id = FakeUpdateLocation.LocationId;
            HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(_client, $"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Set<Covenant.Common.Entities.Agency.AgencyLocation>().SingleAsync(c => c.LocationId == id);
            Assert.Equal(model.Address, entity.Location.Address);
            Assert.Equal(model.PostalCode, entity.Location.PostalCode);
            Assert.Equal(model.City.Id, entity.Location.City.Id);
            Assert.Equal(model.IsBilling, entity.IsBilling);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<List<LocationDetailModel>>();
            Assert.NotEmpty(list);
            var entity = FakeLocation;
            Assert.NotNull(list.Single(c =>
                c.Id == entity.LocationId &&
                c.Address == entity.Location.Address &&
                c.PostalCode == entity.Location.PostalCode &&
                c.City.Value == entity.Location.City.Value &&
                c.City.Province.Value == entity.Location.City.Province.Value &&
                c.City.Province.Country.Id == entity.Location.City.Province.Country.Id &&
                c.IsBilling == entity.IsBilling &&
                c.Latitude.Equals(entity.Location.Latitude) &&
                c.Longitude.Equals(entity.Location.Longitude)));
        }

        [Fact]
        public async Task GetById()
        {
            var entity = FakeLocation;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.LocationId}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<LocationDetailModel>();
            Assert.Equal(entity.LocationId, model.Id);
            Assert.Equal(entity.Location.Address, model.Address);
            Assert.Equal(entity.Location.PostalCode, model.PostalCode);
            Assert.Equal(entity.Location.City.Id, model.City.Id);
            Assert.Equal(entity.Location.City.Value, model.City.Value);
            Assert.Equal(entity.Location.Entrance, model.Entrance);
            Assert.Equal(entity.Location.MainIntersection, model.MainIntersection);
            Assert.Equal(entity.IsBilling, model.IsBilling);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = FakeDeleteLocation.LocationId;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.Set<CompanyProfileLocation>().AnyAsync(c => c.LocationId == id));
        }

        private void Seed(CovenantContext context)
        {
            context.City.AddRange(Toronto, York, Brampton);
            FakeLocation.Location.UpdateCoordinates(43.71153872193341, -79.37071707699555);
            FakeAgency.AddLocation(FakeLocation.Location, true);
            FakeAgency.AddLocation(FakeUpdateLocation.Location);
            FakeAgency.AddLocation(FakeDeleteLocation.Location);
            context.Agencies.Add(FakeAgency);
            context.SaveChanges();
        }
    }
}