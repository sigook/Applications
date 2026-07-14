using Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Company.Models;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.CompanyProfiles
{
    public class ContactInformationControllerTest : IClassFixture<CustomWebApplicationFactory<ContactInformationControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public ContactInformationControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => ContactInformationController.RouteName.Replace("{profileId}",
            Startup.FakeCompanyProfile.Id.ToString());

        [Fact]
        public async Task Put()
        {
            var model = new CompanyProfileDetailModel
            {
                Phone = "645789065",
                PhoneExt = 333,
                Fax = "456789032",
                FaxExt = 555,
                Website = "www.bussines.com"
            };
            HttpResponseMessage response = await _client.PutAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            Guid id = Startup.FakeCompanyProfile.Id;
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            CompanyProfile entity = await context.CompanyProfile.SingleAsync(c => c.Id == id);
            Assert.Equal(model.Phone, entity.Phone);
            Assert.Equal(model.PhoneExt, entity.PhoneExt);
            Assert.Equal(model.Fax, entity.Fax);
            Assert.Equal(model.FaxExt, entity.FaxExt);
            Assert.Equal(model.Website, entity.Website);
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
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency("Test", "Test");
            public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", "", new CompanyProfileIndustry("Company Industry"));

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
                context.CompanyProfile.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}