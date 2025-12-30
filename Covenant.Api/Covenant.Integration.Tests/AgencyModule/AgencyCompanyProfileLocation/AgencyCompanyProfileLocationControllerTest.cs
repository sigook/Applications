using Covenant.Api.AgencyModule.AgencyCompanyProfileLocation.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCompanyProfileLocation
{
    public class AgencyCompanyProfileLocationControllerTest : IClassFixture<CustomWebApplicationFactory<AgencyCompanyProfileLocationControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyCompanyProfileLocationControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyCompanyProfileLocationController.RouteName.Replace("{profileId}",
            Startup.FakeCompanyProfile.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new CompanyProfileLocationDetailModel
            {
                Address = "419 Bloor Ave",
                PostalCode = "A1A1A1",
                Entrance = "Main entrance",
                City = new CityModel(Startup.Toronto.Id, Startup.Toronto.Value),
                IsBilling = true,
                MainIntersection = "Bloor & Dundas"
            };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<CompanyProfileLocationDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            CompanyProfileLocation entity = await context.Set<CompanyProfileLocation>().SingleAsync(c => c.LocationId == detail.Id);
            Assert.Equal(model.Address, entity.Location.Address);
            Assert.Equal(model.PostalCode, entity.Location.PostalCode);
            Assert.Equal(model.Entrance, entity.Location.Entrance);
            Assert.Equal(model.City.Id, entity.Location.City.Id);
            Assert.Equal(model.IsBilling, entity.IsBilling);
            Assert.Equal(model.MainIntersection, entity.Location.MainIntersection);
        }

        [Fact]
        public async Task Put()
        {
            var model = new CompanyProfileLocationDetailModel
            {
                Address = "654 Bloor Update",
                PostalCode = "C4C4C4",
                Entrance = "Update entrance",
                City = new CityModel(Startup.Brampton.Id, Startup.Brampton.Value),
                IsBilling = false,
                MainIntersection = "Bloor & Dundas updated"
            };
            var id = Startup.FakeUpdateLocation.LocationId;
            var response = await HttpClientJsonExtensions.PutAsJsonAsync(_client, $"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Set<CompanyProfileLocation>().SingleAsync(c => c.LocationId == id);
            Assert.Equal(model.Address, entity.Location.Address);
            Assert.Equal(model.PostalCode, entity.Location.PostalCode);
            Assert.Equal(model.Entrance, entity.Location.Entrance);
            Assert.Equal(model.City.Id, entity.Location.City.Id);
            Assert.Equal(model.IsBilling, entity.IsBilling);
            Assert.Equal(model.MainIntersection, entity.Location.MainIntersection);
        }

        [Fact]
        public async Task Get()
        {
            var response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<List<LocationDetailModel>>();
            Assert.NotEmpty(list);
            var entity = Startup.FakeLocation;
            var model = list.Single(c =>
                c.Id == entity.LocationId &&
                c.Address == entity.Location.Address &&
                c.PostalCode == entity.Location.PostalCode &&
                c.City.Value == entity.Location.City.Value &&
                c.City.Province.Value == entity.Location.City.Province.Value &&
                c.City.Province.Country.Id == entity.Location.City.Province.Country.Id &&
                c.IsBilling == entity.IsBilling &&
                c.Entrance == entity.Location.Entrance &&
                c.MainIntersection == entity.Location.MainIntersection &&
                c.Latitude.Equals(entity.Location.Latitude) &&
                c.Longitude.Equals(entity.Location.Longitude));
            Assert.NotNull(model);
        }

        [Fact]
        public async Task GetById()
        {
            CompanyProfileLocation entity = Startup.FakeLocation;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.LocationId}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<CompanyProfileLocationDetailModel>();
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
            Guid id = Startup.FakeDeleteLocation.LocationId;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.Set<CompanyProfileLocation>().AnyAsync(c => c.LocationId == id));
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o => o.AddAgencyPersonnelRole());
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Province Ontario = new Province { Value = "Ontario", Country = new Country { Value = "Canada" } };
            public static readonly City Toronto = new City { Value = "Toronto", Province = Ontario };
            private static readonly City York = new City { Value = "York", Province = Ontario };
            public static readonly City Brampton = new City { Value = "Brampton", Province = Ontario };

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", "", new CompanyProfileIndustry("Company Industry"));

            public static readonly CompanyProfileLocation FakeLocation = new CompanyProfileLocation(FakeCompanyProfile.Id,
                Location.Create(York.Id, "567 Dundas", "A2A2A2", "Back Door", "Dundas & bloor").Value)
            { IsBilling = true };

            public static readonly CompanyProfileLocation FakeUpdateLocation = new CompanyProfileLocation(FakeCompanyProfile.Id,
                Location.Create(York.Id, "543 Dundas update", "B3B3B3", "Dundas & bloor").Value);

            public static readonly CompanyProfileLocation FakeDeleteLocation = new CompanyProfileLocation(FakeCompanyProfile.Id,
                Location.Create(York.Id, "345 Dundas Delete", "C3C3C3", "Dundas & bloor").Value);

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
                context.City.AddRange(Toronto, York, Brampton);
                FakeLocation.Location.UpdateCoordinates(43.71153872193341, -79.37071707699555);
                FakeCompanyProfile.Locations.Add(FakeLocation);
                FakeCompanyProfile.Locations.Add(FakeUpdateLocation);
                FakeCompanyProfile.Locations.Add(FakeDeleteLocation);
                context.CompanyProfile.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}