using Covenant.Api.Controllers.Sigook.Account;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Security;
using Covenant.Common.Resources;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.AccountModule;

public class UserAccountControllerEmailTest(CustomWebApplicationFactory<UserAccountControllerEmailTest.Startup> factory)
    : IClassFixture<CustomWebApplicationFactory<UserAccountControllerEmailTest.Startup>>
{
    private static string RequestUri() => $"{UserAccountController.RouteName}/{nameof(UserAccountController.ChangeEmail)}";

    private HttpClient ClientFor(Guid userId)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, userId.ToString());
        return client;
    }

    private static ChangeEmailModel Model(string email, string confirm = null) =>
        new() { NewEmail = email, ConfirmNewEmail = confirm ?? email };

    [Fact]
    public async Task ChangeEmail()
    {
        const string newEmail = "changed.email@mail.com";
        var response = await ClientFor(Startup.CurrentUser.Id).PostAsJsonAsync(RequestUri(), Model(newEmail));
        response.EnsureSuccessStatusCode();

        var context = factory.Services.GetRequiredService<CovenantContext>();
        var user = await context.Users.AsNoTracking().FirstAsync(u => u.Id == Startup.CurrentUser.Id);
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public async Task ChangeEmail_FailsWhenConfirmationDoesNotMatch()
    {
        var response = await ClientFor(Startup.CurrentUser.Id).PostAsJsonAsync(RequestUri(), Model("a.email@mail.com", "b.email@mail.com"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeEmail_FailsWhenEmailIsTaken()
    {
        var response = await ClientFor(Startup.CurrentUser.Id).PostAsJsonAsync(RequestUri(), Model(Startup.OtherUser.Email));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(ApiResources.EmailAlreadyTaken, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ChangeEmail_FailsWhenEmailIsEqualToCurrent()
    {
        var response = await ClientFor(Startup.OtherUser.Id).PostAsJsonAsync(RequestUri(), Model(Startup.OtherUser.Email));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(UserAccountService.EmailEqualToCurrent, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ChangeEmail_FailsWhenUserDoesNotExist()
    {
        var response = await ClientFor(Guid.NewGuid()).PostAsJsonAsync(RequestUri(), Model("ghost.email@mail.com"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(UserAccountService.UserNotFound, await response.Content.ReadAsStringAsync());
    }

    public class Startup
    {
        public static readonly User CurrentUser = new(CvnEmail.Create("current.user@mail.com").Value);
        public static readonly User OtherUser = new(CvnEmail.Create("taken.user@mail.com").Value);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o => o.AddWorkerRole());
            services.AddTestDatabase();
            services.AddSingleton<IUserAccountService, UserAccountService>();
        }

        public void Configure(IApplicationBuilder app, CovenantContext context)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            context.Users.AddRange(CurrentUser, OtherUser);
            context.SaveChanges();
        }
    }
}
