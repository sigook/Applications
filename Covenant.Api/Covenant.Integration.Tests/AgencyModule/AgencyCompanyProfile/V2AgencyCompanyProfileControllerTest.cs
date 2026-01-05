using Covenant.Api.AgencyModule.AgencyCompanyProfile.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Company.Models;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCompanyProfile
{
    public class V2AgencyCompanyProfileControllerTest : IClassFixture<CustomWebApplicationFactory<V2AgencyCompanyProfileControllerTest.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public V2AgencyCompanyProfileControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri(Guid? id = null) => V2AgencyCompanyProfileController.RouteName + (id is null ? "" : $"/{id}");

        [Fact]
        public async Task Post()
        {
            var model = CreateModel();
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<CompanyProfileDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            CompanyProfile entity = await context.CompanyProfile.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(model.FullName, entity.FullName);
            Assert.Equal(model.Phone, entity.Phone);
            Assert.Equal(model.PhoneExt, entity.PhoneExt);
            Assert.Equal(model.Fax, entity.Fax);
            Assert.Equal(model.FaxExt, entity.FaxExt);
            Assert.Equal(model.Email, entity.Company.Email);
            Assert.Equal(model.Website, entity.Website);
            Assert.Equal(model.Logo?.FileName ?? "company.png", entity.Logo.FileName);
            Assert.Equal(model.Industry.Industry.Id, entity.Industry.IndustryId);
            Assert.Equal(model.Industry.OtherIndustry, entity.Industry.OtherIndustry);
            Assert.Equal(model.About, entity.About);
        }

        private static CompanyProfileDetailModel CreateModel()
        {
            return new CompanyProfileDetailModel
            {
                FullName = "ABC",
                Phone = "654897654",
                PhoneExt = 333,
                Fax = "6437890665",
                FaxExt = 222,
                Website = "www.abc.com",
                Email = "email@test.com",
                Industry = new CompanyProfileIndustryDetailModel { Industry = new BaseModel<Guid>(Startup.Drivers.Id) },
                Logo = new CovenantFileModel("myLogo.png", "Logo"),
                About = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
            };
        }

        [Fact]
        public async Task Get()
        {
            Guid id = Startup.FakeCompanyProfile.Id;
            HttpResponseMessage response = await _client.GetAsync(RequestUri(id));
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<CompanyProfileDetailModel>();
            Assert.NotNull(model);
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            CompanyProfile entity = await context.CompanyProfile.SingleAsync(c => c.Id == id);
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(entity.NumberId, model.NumberId);
            Assert.Equal(entity.FullName, model.FullName);
            Assert.Equal(entity.Phone, model.Phone);
            Assert.Equal(entity.PhoneExt, model.PhoneExt);
            Assert.Equal(entity.Fax, model.Fax);
            Assert.Equal(entity.FaxExt, model.FaxExt);
            Assert.Equal(entity.Company.Email, model.Email);
            Assert.Equal(entity.Website, model.Website);
            Assert.Equal(entity.About, model.About);
            Assert.Equal(entity.Logo.Id, model.Logo.Id);
            Assert.Equal(entity.Logo.FileName, model.Logo.FileName);
            Assert.Equal(entity.Industry.Id, model.Industry.Id);
            Assert.Equal(entity.Industry?.Industry?.Id, model.Industry.Industry?.Id);
            Assert.Equal(entity.Industry?.Industry?.Value, model.Industry?.Industry?.Value);
            Assert.Equal(entity.Industry.OtherIndustry, model.Industry.OtherIndustry);
            Assert.Equal(entity.Active, model.Active);
            Assert.Equal(entity.PaidHolidays, model.PaidHolidays);
            Assert.Equal(entity.RequiredPaymentMethod, model.RequiredPaymentMethod);
            Assert.Equal(entity.VaccinationRequired, model.VaccinationRequired);
            Assert.Equal(entity.VaccinationRequiredComments, model.VaccinationRequiredComments);
        }

        [Fact]
        public async Task VaccinationRequired()
        {
            var model = new VaccinationRequiredModel { Required = true, Comments = "Workers must be fully vaccinated" };
            Guid id = Startup.FakeCompanyProfile.Id;
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri(id)}/VaccinationRequired", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            CompanyProfile entity = await context.CompanyProfile.SingleAsync(s => s.Id == id);
            Assert.Equal(model.Required, entity.VaccinationRequired);
            Assert.Equal(model.Comments, entity.VaccinationRequiredComments);
        }

        public class Startup
        {
            private static readonly DateTime FakeNow = new DateTime(2021, 01, 01);

            public static readonly Industry Drivers = new Industry { Value = "Drivers" };

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency("Test", "Test");

            public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "ABC Corporation", "ABC", "657890764", new CompanyProfileIndustry("Company Industry"));

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole(FakeAgency.Id);
                    });
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<IAgencyService, AgencyService>();
                var timeService = new Mock<ITimeService>();
                timeService.Setup(c => c.GetCurrentDateTime()).Returns(FakeNow);
                services.AddSingleton(timeService.Object);
                var identityServerService = new Mock<IIdentityServerService>();
                identityServerService.Setup(c => c.CreateUser(It.IsAny<CreateUserModel>())).ReturnsAsync(Result.Ok(new User("email@test.com", Guid.NewGuid())));
                identityServerService.Setup(c => c.GetAgencyId()).Returns(FakeAgency.Id);
                services.AddSingleton(identityServerService.Object);
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
                FakeAgency.User = new User(CvnEmail.Create("c@mail.com").Value);
                context.Industry.Add(Drivers);
                FakeCompanyProfile.Logo = new CovenantFile("logo.pdf", "Logo");
                FakeCompanyProfile.UpdateVaccinationInfo(false, "some comment");
                context.CompanyProfile.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}