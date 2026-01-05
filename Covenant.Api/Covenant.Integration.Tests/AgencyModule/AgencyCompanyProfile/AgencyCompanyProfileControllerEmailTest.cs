using Covenant.Api.AgencyModule.AgencyCompanyProfile.Controllers;
using Covenant.Api.AgencyModule.AgencyWorkerProfile.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;
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
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCompanyProfile
{
    public class AgencyCompanyProfileControllerEmailTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AgencyCompanyProfileControllerEmailTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private static string RequestUri() => V2AgencyCompanyProfileController.RouteName;

        public AgencyCompanyProfileControllerEmailTest(CustomWebApplicationFactory<Startup> factory) => _factory = factory;

        [Fact]
        public async Task Email()
        {
            CvnEmail newEmail = CvnEmail.Create("newCoEmail@mail.com").Value;
            HttpClient client = _factory.CreateClient();
            Guid id = Startup.FakeCompany.Id;
            var url = $"{RequestUri()}/{id}/{nameof(V2AgencyCompanyProfileController.Email)}";
            HttpResponseMessage response = await client.PutAsJsonAsync(url, new UpdateEmailModel { NewEmail = newEmail });
            response.EnsureSuccessStatusCode();

            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            User updatedUser = await context.User.FirstAsync(f => f.Id == Startup.FakeCompany.CompanyId);
            Assert.Equal(newEmail.Email, updatedUser.Email);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole(FakeAgencyUser.Id);
                    });
                services.AddHttpClient();
                services.AddDbContext<CovenantContext>(b =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IAgencyService, AgencyService>();
                var identityServerService = new Mock<IIdentityServerService>();
                identityServerService.Setup(m => m.UpdateUserEmail(It.IsAny<UpdateEmailModel>())).ReturnsAsync(Result.Ok());
                identityServerService.Setup(c => c.CreateUser(It.IsAny<CreateUserModel>())).ReturnsAsync(Result.Ok(new User("email@test.com", Guid.NewGuid())));
                services.AddSingleton(identityServerService.Object);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly CvnEmail OtherUserEmail = CvnEmail.Create("other.user@mail.com").Value;
            private static readonly User FakeAgencyUser = new User(CvnEmail.Create($"agency{Guid.NewGuid():N}@email.com").Value);

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency("AG", "AG_NUMBER");

            public static readonly CompanyProfile FakeCompany = CompanyProfile.AgencyCreateCompany(
                new User(CvnEmail.Create("updateMyEmail@e.com").Value),
                FakeAgencyUser.Id,
                CompanyName.Create("COM").Value,
                CompanyName.Create("COM").Value,
                "6478907654",
                default,
                default,
                default,
                default,
                new CompanyProfileIndustry("Other"),
                default,
                default,
                default,
                default,
                "juan@covenantgroupl.com",
                CompanyStatus.Lead,
                null).Value;

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
                context.Agencies.Add(FakeAgency);
                context.CompanyProfile.AddRange(FakeCompany);
                context.User.AddRange(new User(OtherUserEmail));
                context.SaveChanges();
            }
        }
    }
}