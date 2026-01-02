using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Models.Security;
using Covenant.IdentityServer.Controllers.Account.Models;
using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Entities;
using Covenant.IdentityServer.Tests.Utils;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Covenant.IdentityServer.Tests
{
    public class UserAdministrationControllerDeleteAgencyPersonnelTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public UserAdministrationControllerDeleteAgencyPersonnelTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            {
                s.DisableAuthorization();
                s.AddDbContext<MyKeysContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                    .AddDataProtection().PersistKeysToDbContext<MyKeysContext>();
                s.AddDbContext<CovenantContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            }));
            _client = _factory.CreateClient();
        }
        private static readonly Guid FakeAgencyId = Guid.NewGuid();
        private const string RequestUri = "UserAdministration";

        [Fact]
        public async Task DeleteAgencyPersonnel()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser user = await CreateUserToDelete(userManager, roleManager);

                var model = new DeleteUserModel { ClaimId = FakeAgencyId, Id = user.Id };

                HttpResponseMessage response = await SendDeleteRequest(model);
                response.EnsureSuccessStatusCode();

                CovenantUser mustBeNull = await userManager.FindByIdAsync(user.Id.ToString());
                Assert.Null(mustBeNull);
            }
        }

        [Fact]
        public async Task DeleteAgencyPersonnel_IfTheUserBelongsToMoreThanOneAgencyOnlyDeleteClaim()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser user = await CreateUserToDelete(userManager, roleManager);
                Claim otherAgency = Constants.ClaimAgencyId(Guid.NewGuid());
                await userManager.AddClaimAsync(user, otherAgency);

                var model = new DeleteUserModel { ClaimId = FakeAgencyId, Id = user.Id };
                HttpResponseMessage response = await SendDeleteRequest(model);
                response.EnsureSuccessStatusCode();

                Assert.NotNull(await userManager.FindByIdAsync(user.Id.ToString()));
                IList<Claim> claims = await userManager.GetClaimsAsync(user);
                Assert.DoesNotContain(claims, a => a.Value == FakeAgencyId.ToString());
                Assert.Contains(claims, a => a.Value == otherAgency.Value);
            }
        }

        [Fact]
        public async Task DeleteAgencyPersonnel_TheUserShouldBelongToTheAgency()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                CovenantUser user = await CreateUserToDelete(userManager, roleManager);

                var otherAgencyId = Guid.NewGuid();
                var model = new DeleteUserModel { ClaimId = otherAgencyId, Id = user.Id };

                HttpResponseMessage response = await SendDeleteRequest(model);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                string errorMessage = await response.Content.ReadAsStringAsync();
                Assert.Contains("The user doesn't belong to the agency", errorMessage);

                user = await userManager.FindByIdAsync(user.Id.ToString());
                Assert.NotNull(user);
            }
        }

        [Fact]
        public async Task DeleteAgencyPersonnel_TheUserShouldHaveTheRoleAgencyPersonnel()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                var user = await CreateUserToDelete(userManager, roleManager, "OtherRole");

                var model = new DeleteUserModel { ClaimId = FakeAgencyId, Id = user.Id };

                HttpResponseMessage response = await SendDeleteRequest(model);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                string errorMessage = await response.Content.ReadAsStringAsync();
                Assert.Contains("The user is not an agency.personnel", errorMessage);

                user = await userManager.FindByIdAsync(user.Id.ToString());
                Assert.NotNull(user);
            }
        }

        private async Task<HttpResponseMessage> SendDeleteRequest(DeleteUserModel model)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, $"{RequestUri}/{model.Id}");
            message.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json);
            HttpResponseMessage response = await _client.SendAsync(message);
            return response;
        }

        private static async Task<CovenantUser> CreateUserToDelete(UserManager<CovenantUser> userManager,
            RoleManager<CovenantRole> roleManager, string userRole = CovenantConstants.Role.AgencyPersonnel)
        {
            var existingUser = new CovenantUser { Id = Guid.NewGuid(), Email = "existing.personnel@sigook.com", UserName = "existing.personnel@sigook.com" };
            await userManager.CreateAsync(existingUser, "P@ssw0rd");
            await userManager.AddClaimAsync(existingUser, Constants.ClaimAgencyId(FakeAgencyId));
            var role = new CovenantRole { Id = Guid.NewGuid(), Name = userRole };
            await roleManager.CreateAsync(role);
            await userManager.AddToRoleAsync(existingUser, role.Name);
            return existingUser;
        }
    }
}