using Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Company;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.CompanyProfiles
{
    public class LogoControllerTest : IClassFixture<CustomWebApplicationFactory<LogoControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public LogoControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => LogoController.RouteName.Replace("{profileId}",
            Startup.FakeCompanyProfile.Id.ToString());

        [Fact]
        public async Task Put()
        {
            Guid id = Startup.FakeCompanyProfile.Id;
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            CompanyProfile entity = await context.CompanyProfiles.SingleAsync(c => c.Id == id);
            Assert.Equal(CompanyProfile.DefaultImageLogo, entity.Logo.FileName);

            var model = new CovenantFileModel("myLogo.png", "Logo");
            HttpResponseMessage response = await _client.PutAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();

            entity = await context.CompanyProfiles.SingleAsync(c => c.Id == id);
            Assert.NotNull(entity.LogoId);
            Assert.Equal(model.FileName, entity.Logo.FileName);
            Assert.Equal(model.Description, entity.Logo.Description);

            model = new CovenantFileModel("myNewLogo.png", "NewLogo");
            response = await _client.PutAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();

            entity = await context.CompanyProfiles.SingleAsync(c => c.Id == id);
            Assert.NotNull(entity.LogoId);
            Assert.Equal(model.FileName, entity.Logo.FileName);
            Assert.Equal(model.Description, entity.Logo.Description);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole(FakeAgency.Id);
                    });
                services.AddTestDatabase();
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency("Test", "Test") { User = FakeData.FakeUser() };
            public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", new CompanyProfileIndustry("Company Industry"));

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
                FakeAgency.User = new User(CvnEmail.Create("agency@mail.com").Value);
                context.CompanyProfiles.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}