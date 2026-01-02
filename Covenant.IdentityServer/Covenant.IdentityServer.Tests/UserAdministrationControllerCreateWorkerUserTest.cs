using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;
using Covenant.Common.Utils.Extensions;
using Covenant.IdentityServer.Controllers.Account.Models;
using Covenant.IdentityServer.Data;
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
    public class UserAdministrationControllerCreateWorkerUserTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private static readonly FakeEmailService FakeEmailService = new FakeEmailService();
        private static readonly FakeRazorViewToStringRenderer FakeRenderer = new FakeRazorViewToStringRenderer();

        public UserAdministrationControllerCreateWorkerUserTest(CustomWebApplicationFactory<Startup> factory)
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
        public async Task CreateWorker()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var model = new CreateUserModel 
                { 
                    Email = "worker@sigook.com", 
                    Password = "Sigook123",
                    UserType = UserType.Worker,
                    ConfirmPassword = "Sigook123"
                };
                var requestUri = "UserAdministration/CreateUser";
                HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, model);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsJsonAsync<IdModel>();

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser user = await userManager.FindByIdAsync(result.Id.ToString());
                Assert.Equal(model.Email, user.Email);
                Assert.Equal(model.Email, user.UserName);

                Claim claim = (await userManager.GetClaimsAsync(user)).Single();
                Assert.Equal(Constants.Worker, claim.Type);
                Assert.Equal(Constants.Worker, claim.Value);

                Assert.Equal(model.Email, FakeEmailService.Email);
                Assert.Equal(Resources.Resources.ConfirmYourAccount, FakeEmailService.Subject);
                Assert.NotNull(FakeEmailService.Message);

                response = await _client.GetAsync(FakeRenderer.ConfirmAccountViewModel.Url);
                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task CreateWorker_WhenUserAlreadyExistsReturnBadRequest()
        {
            using (IServiceScope serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<CovenantUser>>();
                CovenantUser existingUser = await CreateExistingWorker(userManager, false);
                var model = new CreateUserModel { Email = existingUser.Email };
                var requestUri = "UserAdministration/CreateUser";
                HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, model);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        private static async Task<CovenantUser> CreateExistingWorker(UserManager<CovenantUser> userManager, bool isWorker = true)
        {
            var existingUser = new CovenantUser { Id = Guid.NewGuid(), Email = "existing.worker@sigook.com", UserName = "existing.worker@sigook.com" };
            await userManager.CreateAsync(existingUser, "P@ssw0rd");
            if (isWorker) await userManager.AddClaimAsync(existingUser, Constants.WorkerClaim());
            return existingUser;
        }
    }
}