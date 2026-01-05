using Covenant.Api.Authorization;
using Covenant.Api.Utils;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyProfileT
{
    public class TestCompanyProfileController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<TestCompanyProfileController.Startup>>
    {
        private readonly HttpClient _client;

        public TestCompanyProfileController(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Post()
        {
            var model = new CompanyRegisterByItselfModel
            {
                Name = "Company",
                Email = "company@company.com",
                Password = "P@ssw0rd",
                ConfirmPassword = "P@ssw0rd",
                Locations = new List<LocationModel> { new LocationModel { Address = "Address", PostalCode = "A1A1A1", City = new CityModel { Id = Startup.FakeCity.Id } } }
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/CompanyProfile", model);
            response.EnsureSuccessStatusCode();
            response = await _client.GetAsync(response.Headers.Location);
            var detail = await response.Content.ReadAsJsonAsync<CompanyProfileDetailModel>();
            Assert.NotNull(detail);
            Assert.Equal(model.Name, detail.BusinessName);
            Assert.Equal(model.Name, detail.FullName);
            Assert.Equal(model.Email, detail.Email);
            Assert.NotNull(detail.Logo);
        }

        public class Startup
        {
            private static readonly Guid FakeCompanyId = Guid.NewGuid();
            public static readonly City FakeCity = new City { Value = "Toronto", Province = new Province { Country = new Country() } };

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(FakeCompanyId);
                    o.AddCompanyRole();
                });
                services.AddDbContext<CovenantContext>(b =>
                    b.UseInMemoryDatabase(nameof(TestCompanyProfileController)), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                var defaultLogoProvider = new Mock<IDefaultLogoProvider>();
                defaultLogoProvider.Setup(p => p.GetLogo(It.IsAny<string>())).ReturnsAsync(new CovenantFileModel("logo.png", "Logo"));
                services.AddSingleton(defaultLogoProvider.Object);
                services.AddSingleton<ICompanyService, CompanyService>();
                var agencyRepositoryMock = new Mock<IAgencyRepository>();
                agencyRepositoryMock.Setup(ar => ar.GetAgencyMasterByLocation(It.IsAny<CityModel>())).ReturnsAsync(new Covenant.Common.Entities.Agency.Agency());
                services.AddSingleton(agencyRepositoryMock.Object);
                services.AddSingleton<ITimeService, TimeService>();
                var identityServerService = new Mock<IIdentityServerService>();
                identityServerService.Setup(c => c.CreateUser(It.IsAny<CreateUserModel>())).ReturnsAsync(Result.Ok(new User("company@company.com", FakeCompanyId)));
                services.AddSingleton(identityServerService.Object);
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
                context.City.Add(FakeCity);
                context.SaveChanges();
            }
        }
    }
}
