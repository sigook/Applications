using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Covenant.Core.BL.Services.Identity;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Covenant.Integration.Tests.Identity;

public class IdentityControllersTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<IdentityControllersTest.Startup>>
{
    private const string WebClientUrl = "https://app.sigook.test/";

    private readonly HttpClient _client;

    public IdentityControllersTest(CustomWebApplicationFactory<Startup> factory) =>
        _client = factory
            .WithWebHostBuilder(b => b.UseSetting("WebClientUrl", WebClientUrl))
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private static async Task<JsonElement> Json(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    private async Task<string> RedirectOf(string url)
    {
        HttpResponseMessage response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        return response.Headers.Location!.OriginalString;
    }

    [Fact]
    public async Task Home_RedirectsToTheWebClient()
    {
        Assert.Equal("https://app.sigook.test", await RedirectOf("/"));
        Assert.Equal("https://app.sigook.test/login?error=invalid_user", await RedirectOf("/Home/InvalidUser"));
    }

    [Theory]
    [InlineData("/Account/ConfirmEmailAddress", "confirm-email")]
    [InlineData("/Account/CreatePassword", "create-password")]
    [InlineData("/Account/ResetPassword", "create-password")]
    public async Task LegacyEmailLinks_RedirectToTheWebPages(string legacyPath, string webPath)
    {
        var id = Guid.NewGuid();

        string location = await RedirectOf($"{legacyPath}?token=a%2Bb%2F&id={id}");

        Assert.Equal($"https://app.sigook.test/{webPath}?token=a%2Bb%2F&id={id}", location);
    }

    [Fact]
    public async Task ConfirmEmail_ValidToken_ReturnsOk()
    {
        var id = Guid.NewGuid();
        Startup.UserAdministration.Setup(s => s.ConfirmEmail(id, "good")).ReturnsAsync(Result.Ok());

        HttpResponseMessage response = await _client.PostAsJsonAsync("/Account/ConfirmEmail", new ConfirmEmailAddressModel { Id = id, Token = "good" });

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ConfirmEmail_RejectedToken_ReturnsTheInvalidTokenCode()
    {
        var id = Guid.NewGuid();
        Startup.UserAdministration.Setup(s => s.ConfirmEmail(id, "bad")).ReturnsAsync(Result.Fail("Invalid token"));

        HttpResponseMessage response = await _client.PostAsJsonAsync("/Account/ConfirmEmail", new ConfirmEmailAddressModel { Id = id, Token = "bad" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(PasswordResetService.InvalidToken, (await Json(response)).GetProperty("error").GetString());
    }

    [Fact]
    public async Task ConfirmEmail_MissingFields_ReturnsValidationErrors()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/Account/ConfirmEmail", new ConfirmEmailAddressModel());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonElement body = await Json(response);
        Assert.True(body.TryGetProperty("Id", out _));
        Assert.True(body.TryGetProperty("Token", out _));
    }

    [Fact]
    public async Task CreatePassword_Valid_ReturnsOk()
    {
        var id = Guid.NewGuid();
        Startup.PasswordReset.Setup(s => s.CreatePassword(id, "token", "Secret1")).ReturnsAsync(PasswordResetResult.Success());

        HttpResponseMessage response = await _client.PostAsJsonAsync("/Account/CreatePassword", new CreatePasswordModel { Id = id, Token = "token", Password = "Secret1" });

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreatePassword_PolicyFailure_ReturnsErrorCodeAndMessages()
    {
        var id = Guid.NewGuid();
        Startup.PasswordReset.Setup(s => s.CreatePassword(id, "token", "secret1"))
            .ReturnsAsync(PasswordResetResult.Fail(PasswordResetService.PasswordPolicy, ["Passwords must have at least one uppercase ('A'-'Z')."]));

        HttpResponseMessage response = await _client.PostAsJsonAsync("/Account/CreatePassword", new CreatePasswordModel { Id = id, Token = "token", Password = "secret1" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonElement body = await Json(response);
        Assert.Equal(PasswordResetService.PasswordPolicy, body.GetProperty("error").GetString());
        Assert.Equal("Passwords must have at least one uppercase ('A'-'Z').", body.GetProperty("messages")[0].GetString());
    }

    [Fact]
    public async Task CreatePassword_TooShort_IsRejectedBeforeReachingTheService()
    {
        var id = Guid.NewGuid();

        HttpResponseMessage response = await _client.PostAsJsonAsync("/Account/CreatePassword", new CreatePasswordModel { Id = id, Token = "token", Password = "123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Startup.PasswordReset.Verify(s => s.CreatePassword(id, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResendConfirmationLink_SendsTheLink()
    {
        HttpResponseMessage response = await _client.PostAsync("/Account/ResendConfirmationLink?userName=resend@sigook.com", null);

        response.EnsureSuccessStatusCode();
        Startup.Notifications.Verify(n => n.ResendConfirmAccount("resend@sigook.com"), Times.Once);
    }

    [Fact]
    public async Task ResendConfirmationLink_WithoutUserName_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsync("/Account/ResendConfirmationLink", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_ValidEmail_IsAccepted()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/Password/forgot", new ForgotPasswordModel { Email = "forgot@sigook.com" });

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Startup.PasswordReset.Verify(s => s.RequestCode("forgot@sigook.com"), Times.Once);
    }

    [Fact]
    public async Task ForgotPassword_InvalidEmail_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/Password/forgot", new ForgotPasswordModel { Email = "not-an-email" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Startup.PasswordReset.Verify(s => s.RequestCode("not-an-email"), Times.Never);
    }

    [Fact]
    public async Task ResetPassword_ValidCode_ReturnsOk()
    {
        Startup.PasswordReset.Setup(s => s.ResetPassword("reset@sigook.com", "123456", "Secret1")).ReturnsAsync(PasswordResetResult.Success());

        HttpResponseMessage response = await _client.PostAsJsonAsync("/Password/reset",
            new ResetPasswordWithCodeModel { Email = "reset@sigook.com", Code = "123456", NewPassword = "Secret1" });

        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(PasswordResetService.InvalidCode)]
    [InlineData(PasswordResetService.CodeExpired)]
    [InlineData(PasswordResetService.TooManyAttempts)]
    public async Task ResetPassword_Failure_ReturnsTheErrorCode(string error)
    {
        string email = $"{error}@sigook.com";
        Startup.PasswordReset.Setup(s => s.ResetPassword(email, "654321", "Secret1")).ReturnsAsync(PasswordResetResult.Fail(error));

        HttpResponseMessage response = await _client.PostAsJsonAsync("/Password/reset",
            new ResetPasswordWithCodeModel { Email = email, Code = "654321", NewPassword = "Secret1" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(error, (await Json(response)).GetProperty("error").GetString());
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("abcdef")]
    public async Task ResetPassword_MalformedCode_ReturnsBadRequest(string code)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/Password/reset",
            new ResetPasswordWithCodeModel { Email = "malformed@sigook.com", Code = code, NewPassword = "Secret1" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Startup.PasswordReset.Verify(s => s.ResetPassword("malformed@sigook.com", It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    public class Startup
    {
        public static readonly Mock<IUserAdministrationService> UserAdministration = new();
        public static readonly Mock<IPasswordResetService> PasswordReset = new();
        public static readonly Mock<IAccountNotificationService> Notifications = new();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
            services.AddTestDatabase();
            services.AddSingleton(UserAdministration.Object);
            services.AddSingleton(PasswordReset.Object);
            services.AddSingleton(Notifications.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
