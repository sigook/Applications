using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Companies.Profile;
using Covenant.Api.Utils;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Identity;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Companies;
using Covenant.Core.BL.Services.Companies;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Companies;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Companies.Profile;

public class CompanyControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CompanyControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public CompanyControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

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
        HttpResponseMessage response = await _client.PostAsJsonAsync(CompanyController.RouteName, model);
        response.EnsureSuccessStatusCode();
        response = await _client.GetAsync(response.Headers.Location);
        var detail = await response.Content.ReadFromJsonAsync<CompanyProfileDetailModel>();
        Assert.NotNull(detail);
        Assert.Equal(model.Name, detail.FullName);
        Assert.Equal(model.Email, detail.Email);
        Assert.NotNull(detail.Logo);
        Assert.Equal(CompanyProfile.DefaultImageLogo, detail.Logo.FileName);
    }

    public class Startup
    {
        private static readonly Guid FakeCompanyId = Guid.NewGuid();
        public static readonly City FakeCity = new City { Value = "Toronto", Province = new Province { Country = FakeData.FakeCountry("CA") } };

        public static readonly Agency MasterAgency = FakeData.FakeAgency();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o =>
            {
                o.AddSub(FakeCompanyId);
                o.AddCompanyRole();
            });
            services.AddTestDatabase();
            services.AddSingleton<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<ICompanyService, CompanyService>();
            var agencyRepositoryMock = new Mock<IAgencyRepository>();
            agencyRepositoryMock.Setup(ar => ar.GetAgencyMasterByLocation(It.IsAny<CityModel>())).ReturnsAsync(MasterAgency);
            services.AddSingleton(agencyRepositoryMock.Object);
            var userAccountService = new Mock<IUserAccountService>();
            userAccountService.Setup(c => c.CreateUser(It.IsAny<CreateUserModel>())).ReturnsAsync(Result.Ok(new User("company@company.com", FakeCompanyId)));
            services.AddSingleton(userAccountService.Object);
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
            context.Agencies.Add(MasterAgency);
            context.Cities.Add(FakeCity);
            context.SaveChanges();
        }
    }
}
