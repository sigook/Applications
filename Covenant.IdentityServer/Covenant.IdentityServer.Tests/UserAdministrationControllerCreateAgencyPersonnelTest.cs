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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Covenant.IdentityServer.Tests
{
    public class UserAdministrationControllerCreateAgencyPersonnelTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private static readonly FakeRazorViewToStringRenderer FakeRenderer = new FakeRazorViewToStringRenderer();
        private static readonly Guid CovenantId = Guid.NewGuid();

        public UserAdministrationControllerCreateAgencyPersonnelTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(b =>
            {
                b.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "CovenantId", CovenantId.ToString() }
                }));
                b.ConfigureServices(s =>
                {
                    s.DisableAuthorization();
                    s.AddDbContext<MyKeysContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                        .AddDataProtection().PersistKeysToDbContext<MyKeysContext>();
                    s.AddDbContext<CovenantContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                }).ConfigureTestServices(s =>
                {
                    s.AddScoped<IRazorViewToStringRenderer>(c => FakeRenderer);
                });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateAgencyPersonnel()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                CovenantRole role = await CreateRequiredRole(roleManager);
                var model = new CreateUserModel 
                { 
                    Email = "personnel@mail.com", 
                    AgencyId = Guid.NewGuid(),
                    UserType = UserType.AgencyPersonnel,
                    Password = "Sigook123"
                };
                var requestUri = "UserAdministration/CreateUser";
                HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, model);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsJsonAsync<IdModel>();

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser user = await userManager.FindByIdAsync(result.Id.ToString());
                Assert.Equal(model.Email, user.Email);
                Assert.Equal(model.Email, user.UserName);
                Assert.True(user.EmailConfirmed);

                Assert.True(await userManager.IsInRoleAsync(user, role.Name));
                Claim claim = (await userManager.GetClaimsAsync(user)).Single();
                Assert.Equal(Constants.AgencyId, claim.Type);
                Assert.Equal(model.AgencyId.ToString(), claim.Value);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task CreateAgencyPersonnel_UserCanBeAddedToMoreThanOneAgency()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                await CreateRequiredRole(roleManager);
                var modelAgency1 = new CreateUserModel 
                { 
                    Email = "personnel@mail.com", 
                    AgencyId = Guid.NewGuid(),
                    UserType = UserType.AgencyPersonnel,
                    Password = "Sigook123"
                };
                var requestUri = "UserAdministration/CreateUser";
                HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, modelAgency1);
                response.EnsureSuccessStatusCode();
                var newUser = await response.Content.ReadAsJsonAsync<IdModel>();
                requestUri = $"UserAdministration/UpdateAgencyUser/{newUser.Id}";
                var modelAgency2 = new IdModel { Id = Guid.NewGuid() };
                response = await _client.PutAsJsonAsync(requestUri, modelAgency2);
                response.EnsureSuccessStatusCode();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser user = await userManager.FindByIdAsync(newUser.Id.ToString());
                IList<Claim> claims = await userManager.GetClaimsAsync(user);
                Assert.Contains(claims, a => a.Value == modelAgency1.AgencyId.ToString());
                Assert.Contains(claims, a => a.Value == modelAgency2.Id.ToString());
            }
        }

        [Fact]
        public async Task CreateAgencyPersonnel_InvalidModelReturnsBadRequest()
        {
            var requestUri = "UserAdministration/CreateUser";
            HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, new { });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var missingEmailAndAgencyId = new CreateUserModel();
            response = await _client.PostAsJsonAsync(requestUri, missingEmailAndAgencyId);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private static async Task<CovenantRole> CreateRequiredRole(RoleManager<CovenantRole> roleManager)
        {
            var role = new CovenantRole { Id = Guid.NewGuid(), Name = CovenantConstants.Role.AgencyPersonnel };
            await roleManager.CreateAsync(role);
            return role;
        }

        private static async Task<CovenantUser> CreateExistingUser(UserManager<CovenantUser> userManager)
        {
            var existingUser = new CovenantUser { Id = Guid.NewGuid(), Email = "existing.personnel@sigook.com", UserName = "existing.personnel@sigook.com" };
            await userManager.CreateAsync(existingUser, "P@ssw0rd");
            return existingUser;
        }
    }
}