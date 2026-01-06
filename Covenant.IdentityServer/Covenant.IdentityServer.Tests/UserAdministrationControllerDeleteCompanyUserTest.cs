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
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Covenant.IdentityServer.Tests
{
    public class UserAdministrationControllerDeleteCompanyUserTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public UserAdministrationControllerDeleteCompanyUserTest(CustomWebApplicationFactory<Startup> factory)
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
        private static readonly Guid CompanyId = Guid.NewGuid();
        private const string RequestUri = "UserAdministration";

        [Fact]
        public async Task DeleteCompanyUser()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser user = await CreateUserToDelete(userManager, roleManager);

                var model = new DeleteUserModel { ClaimId = CompanyId, Id = user.Id };

                HttpResponseMessage response = await SendDeleteRequest(model);
                response.EnsureSuccessStatusCode();

                CovenantUser mustBeNull = await userManager.FindByIdAsync(user.Id.ToString());
                Assert.Null(mustBeNull);
            }
        }

        [Fact]
        public async Task DeleteCompanyUser_TheUserShouldBelongToTheCompany()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                CovenantUser user = await CreateUserToDelete(userManager, roleManager);

                var otherCompanyId = Guid.NewGuid();
                var model = new DeleteUserModel { ClaimId = otherCompanyId, Id = user.Id };

                HttpResponseMessage response = await SendDeleteRequest(model);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                string errorMessage = await response.Content.ReadAsStringAsync();
                Assert.Contains("The user doesn't belong to the company", errorMessage);

                user = await userManager.FindByIdAsync(user.Id.ToString());
                Assert.NotNull(user);
            }
        }

        [Fact]
        public async Task DeleteCompanyUser_TheUserShouldHaveTheRoleCompanyUser()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<CovenantRole>>();
                CovenantUser user = await CreateUserToDelete(userManager, roleManager, Constants.Agency);

                var model = new DeleteUserModel { ClaimId = CompanyId, Id = user.Id };

                HttpResponseMessage response = await SendDeleteRequest(model);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                string errorMessage = await response.Content.ReadAsStringAsync();
                Assert.Contains("The user is not a company.user", errorMessage);

                user = await userManager.FindByIdAsync(user.Id.ToString());
                Assert.NotNull(user);
            }
        }

        private async Task<HttpResponseMessage> SendDeleteRequest(DeleteUserModel model)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{RequestUri}/{model.Id}");
            message.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json);
            HttpResponseMessage response = await _client.SendAsync(message);
            return response;
        }

        private static async Task<CovenantUser> CreateUserToDelete(UserManager<CovenantUser> userManager,
            RoleManager<CovenantRole> roleManager, string userRole = CovenantConstants.Role.CompanyUser)
        {
            var existingUser = new CovenantUser { Id = Guid.NewGuid(), Email = "existing.user@sigook.com", UserName = "existing.user@sigook.com" };
            await userManager.CreateAsync(existingUser, "P@ssw0rd");
            await userManager.AddClaimAsync(existingUser, new Claim(Constants.CompanyId, CompanyId.ToString()));
            var role = new CovenantRole { Id = Guid.NewGuid(), Name = userRole };
            await roleManager.CreateAsync(role);
            await userManager.AddToRoleAsync(existingUser, role.Name);
            return existingUser;
        }
    }
}