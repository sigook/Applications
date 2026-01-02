using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;
using Covenant.Common.Utils.Extensions;
using Covenant.IdentityServer.Controllers.Account.Models;
using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Entities;
using Covenant.IdentityServer.Services;
using Covenant.IdentityServer.Tests.Utils;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Covenant.IdentityServer.Tests
{
    public class UserAdministrationControllerCreateCompanyUserTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private static readonly FakeEmailService FakeEmailService = new FakeEmailService();
        private static readonly FakeRazorViewToStringRenderer FakeRenderer = new FakeRazorViewToStringRenderer();

        public UserAdministrationControllerCreateCompanyUserTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            {
                s.DisableAuthorization();
                s.AddDbContext<MyKeysContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                    .AddDataProtection().PersistKeysToDbContext<MyKeysContext>();
                s.AddDbContext<CovenantContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            }).ConfigureTestServices(s =>
            {
                s.AddScoped<IEmailService>(c => FakeEmailService);
                s.AddScoped<IRazorViewToStringRenderer>(c => FakeRenderer);
            }));
            _client = _factory.CreateClient();
        }



        [Fact]
        public async Task CreateCompanyUser()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                CovenantRole role = await CreateRequiredRole(roleManager);
                var model = new CreateUserModel 
                { 
                    Email = "e@mail.com", 
                    CompanyId = Guid.NewGuid(),
                    UserType = UserType.CompanyUser,
                    Password = "Sigook123"
                };
                string requestUri = "UserAdministration/CreateUser";
                HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, model);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsJsonAsync<IdModel>();

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser user = await userManager.FindByIdAsync(result.Id.ToString());
                Assert.Equal(model.Email, user.Email);
                Assert.Equal(model.Email, user.UserName);
                Assert.False(user.EmailConfirmed);

                Assert.True(await userManager.IsInRoleAsync(user, role.Name));
                Claim claim = (await userManager.GetClaimsAsync(user)).Single();
                Assert.Equal(Constants.CompanyId, claim.Type);
                Assert.Equal(model.CompanyId.ToString(), claim.Value);

                Assert.Equal(model.Email, FakeEmailService.Email);
                Assert.Equal(Resources.Resources.ConfirmYourAccount, FakeEmailService.Subject);
                Assert.NotNull(FakeEmailService.Message);

                Assert.Contains("CreatePassword", FakeRenderer.ConfirmAccountViewModel.Url);
                response = await _client.GetAsync(FakeRenderer.ConfirmAccountViewModel.Url);
                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task CreateCompanyUser_IfEmailIsTakenReturnsBadRequest()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser existingUser = await CreateExistingUser(userManager);
                string requestUri = "UserAdministration/CreateUser";
                var model = new CreateUserModel { Email = existingUser.Email, CompanyId = Guid.NewGuid() };
                HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, model);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                string errorMessage = await response.Content.ReadAsStringAsync();
                Assert.Contains("User already exists", errorMessage);
            }
        }

        private static async Task<CovenantRole> CreateRequiredRole(RoleManager<CovenantRole> roleManager)
        {
            var role = new CovenantRole { Id = Guid.NewGuid(), Name = CovenantConstants.Role.CompanyUser };
            await roleManager.CreateAsync(role);
            return role;
        }

        private static async Task<CovenantUser> CreateExistingUser(UserManager<CovenantUser> userManager)
        {
            var existingUser = new CovenantUser { Id = Guid.NewGuid(), Email = "existing.user@sigook.com", UserName = "existing.user@sigook.com" };
            await userManager.CreateAsync(existingUser, "P@ssw0rd");
            return existingUser;
        }
    }
}