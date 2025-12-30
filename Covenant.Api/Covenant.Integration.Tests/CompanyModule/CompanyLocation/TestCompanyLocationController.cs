using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyLocation
{
    public class TestCompanyLocationController : ControllerBase, IClassFixture<CustomWebApplicationFactory<TestCompanyLocationController.Startup>>
    {
        private readonly HttpClient _client;
        public TestCompanyLocationController(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync("api/CompanyLocation");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<List<LocationDetailModel>>();
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
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
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
                context.CompanyProfile.Add(Data.CompanyProfile);
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
                    new CompanyProfileLocation {Location = new Location {Address = "ABC",City = new City {Province = new Province{Country = new Country()}}}},
                    new CompanyProfileLocation {Location = new Location {Address = "DFG",City = new City {Province = new Province{Country =new Country()}}}}
                }
            };
        }
    }
}