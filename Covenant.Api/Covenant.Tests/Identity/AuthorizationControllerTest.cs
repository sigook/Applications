using Covenant.Api.Controllers.Identity;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Identity;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Repositories.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Covenant.Tests.Identity;

public class AuthorizationControllerTest
{
    private const string Email = "worker@sigook.com";
    private const string Password = "Secret123";

    private readonly Mock<UserManager<CovenantUser>> _userManager = new(Mock.Of<IUserStore<CovenantUser>>(), null, null, null, null, null, null, null, null);
    private readonly Mock<SignInManager<CovenantUser>> _signInManager;
    private readonly Mock<IIdentityRepository> _repository = new();
    private readonly Mock<IUserSessionValidator> _sessionValidator = new();
    private readonly Mock<IOpenIddictApplicationManager> _applications = new();
    private readonly Mock<IOpenIddictScopeManager> _scopes = new();
    private readonly Mock<IAuthenticationService> _authentication = new();
    private readonly CovenantUser _user = new() { Id = Guid.NewGuid(), Email = Email, EmailConfirmed = true };

    public AuthorizationControllerTest()
    {
        _signInManager = new Mock<SignInManager<CovenantUser>>(
            _userManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<CovenantUser>>(), null, null, null, null);
        _userManager.Setup(u => u.FindByEmailAsync(Email)).ReturnsAsync(_user);
        _userManager.Setup(u => u.FindByIdAsync(_user.Id.ToString())).ReturnsAsync(_user);
        _userManager.Setup(u => u.IsEmailConfirmedAsync(_user)).ReturnsAsync(() => _user.EmailConfirmed);
        _userManager.Setup(u => u.GetRolesAsync(_user)).ReturnsAsync([CovenantConstants.Role.Worker]);
        _signInManager.Setup(s => s.CheckPasswordSignInAsync(_user, Password, true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _signInManager.Setup(s => s.CheckPasswordSignInAsync(_user, It.Is<string>(p => p != Password), true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        _signInManager.Setup(s => s.CanSignInAsync(_user)).ReturnsAsync(true);
        _sessionValidator.Setup(s => s.IsActive(_user)).ReturnsAsync(true);
        _scopes.Setup(s => s.ListResourcesAsync(It.IsAny<System.Collections.Immutable.ImmutableArray<string>>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { "covenant-api" }.ToAsyncEnumerable());
    }

    private AuthorizationController Controller(OpenIddictRequest request, ClaimsPrincipal user = null)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddSingleton(_authentication.Object).BuildServiceProvider(),
            User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
        };
        httpContext.Features.Set(new OpenIddictServerAspNetCoreFeature { Transaction = new OpenIddictServerTransaction { Request = request } });
        return new AuthorizationController(
            _userManager.Object,
            _signInManager.Object,
            _repository.Object,
            _sessionValidator.Object,
            _applications.Object,
            _scopes.Object,
            NullLogger<AuthorizationController>.Instance)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
    }

    private static OpenIddictRequest PasswordGrant(string username = Email, string password = Password, params string[] scopes) =>
        new() { GrantType = GrantTypes.Password, Username = username, Password = password, Scope = string.Join(' ', scopes) };

    private static void AssertInvalidGrant(IActionResult result, string description)
    {
        var forbid = Assert.IsType<ForbidResult>(result);
        Assert.Equal([OpenIddictServerAspNetCoreDefaults.AuthenticationScheme], forbid.AuthenticationSchemes);
        Assert.Equal(Errors.InvalidGrant, forbid.Properties!.Items[OpenIddictServerAspNetCoreConstants.Properties.Error]);
        Assert.Equal(description, forbid.Properties.Items[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription]);
    }

    private void RefreshTokenFor(string subject) =>
        _authentication.Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
            .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity([new Claim(Claims.Subject, subject), new Claim(IdentityClaims.Nickname, "Willy")], "test")),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)));

    [Fact]
    public async Task PasswordGrant_UnknownUser_ReturnsInvalidCredentials()
    {
        IActionResult result = await Controller(PasswordGrant(username: "nobody@sigook.com")).Exchange();

        AssertInvalidGrant(result, "invalid_credentials");
    }

    [Fact]
    public async Task PasswordGrant_InactiveUser_ReturnsInactiveUser_WithoutCheckingThePassword()
    {
        _repository.Setup(r => r.IsInactive(_user.Id)).ReturnsAsync(true);

        IActionResult result = await Controller(PasswordGrant()).Exchange();

        AssertInvalidGrant(result, "inactive_user");
        _signInManager.Verify(s => s.CheckPasswordSignInAsync(It.IsAny<CovenantUser>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task PasswordGrant_LockedOut_ReturnsLockedOut()
    {
        _signInManager.Setup(s => s.CheckPasswordSignInAsync(_user, Password, true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        IActionResult result = await Controller(PasswordGrant()).Exchange();

        AssertInvalidGrant(result, "locked_out");
    }

    [Fact]
    public async Task PasswordGrant_WrongPassword_ReturnsInvalidCredentials_AndCountsTheFailure()
    {
        IActionResult result = await Controller(PasswordGrant(password: "wrong")).Exchange();

        AssertInvalidGrant(result, "invalid_credentials");
        _signInManager.Verify(s => s.CheckPasswordSignInAsync(_user, "wrong", true), Times.Once);
    }

    [Fact]
    public async Task PasswordGrant_UnconfirmedEmail_ReturnsEmailNotConfirmed()
    {
        _user.EmailConfirmed = false;

        IActionResult result = await Controller(PasswordGrant()).Exchange();

        AssertInvalidGrant(result, "email_not_confirmed");
    }

    [Fact]
    public async Task PasswordGrant_Valid_SignsInWithTheUserClaims()
    {
        IActionResult result = await Controller(PasswordGrant(Email, Password, Scopes.OfflineAccess, Scopes.Roles)).Exchange();

        var signIn = Assert.IsType<SignInResult>(result);
        Assert.Equal(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, signIn.AuthenticationScheme);
        ClaimsPrincipal principal = signIn.Principal;
        Assert.Equal(_user.Id.ToString(), principal.GetClaim(Claims.Subject));
        Assert.Equal(Email, principal.GetClaim(Claims.Email));
        Assert.Equal(Email, principal.GetClaim(IdentityClaims.Nickname));
        Assert.Equal(IdentityClaims.LocalIdentityProvider, principal.GetClaim(IdentityClaims.IdentityProvider));
        Assert.Equal([CovenantConstants.Role.Worker], principal.GetClaims(Claims.Role).ToArray());
        Assert.Contains(Scopes.OfflineAccess, principal.GetScopes());
        Assert.Equal(["covenant-api"], principal.GetResources().ToArray());
    }

    [Fact]
    public async Task PasswordGrant_RoleReachesTheIdentityTokenOnlyWithTheRolesScope()
    {
        var withRoles = Assert.IsType<SignInResult>(await Controller(PasswordGrant(Email, Password, Scopes.Roles)).Exchange()).Principal;
        var withoutRoles = Assert.IsType<SignInResult>(await Controller(PasswordGrant()).Exchange()).Principal;

        Assert.Contains(Destinations.IdentityToken, withRoles.Claims.Single(c => c.Type == Claims.Role).GetDestinations());
        Assert.Equal([Destinations.AccessToken], withoutRoles.Claims.Single(c => c.Type == Claims.Role).GetDestinations().ToArray());
        Assert.Contains(Destinations.IdentityToken, withoutRoles.Claims.Single(c => c.Type == Claims.Subject).GetDestinations());
    }

    [Fact]
    public async Task RefreshGrant_ActiveUser_IssuesNewTokensKeepingTheNickname()
    {
        RefreshTokenFor(_user.Id.ToString());

        IActionResult result = await Controller(new OpenIddictRequest { GrantType = GrantTypes.RefreshToken }).Exchange();

        ClaimsPrincipal principal = Assert.IsType<SignInResult>(result).Principal;
        Assert.Equal(_user.Id.ToString(), principal.GetClaim(Claims.Subject));
        Assert.Equal("Willy", principal.GetClaim(IdentityClaims.Nickname));
    }

    [Fact]
    public async Task RefreshGrant_DeletedUser_IsRejected()
    {
        RefreshTokenFor(Guid.NewGuid().ToString());

        IActionResult result = await Controller(new OpenIddictRequest { GrantType = GrantTypes.RefreshToken }).Exchange();

        AssertInvalidGrant(result, "The token is no longer valid.");
    }

    [Fact]
    public async Task RefreshGrant_DeactivatedUser_IsRejected()
    {
        RefreshTokenFor(_user.Id.ToString());
        _sessionValidator.Setup(s => s.IsActive(_user)).ReturnsAsync(false);

        IActionResult result = await Controller(new OpenIddictRequest { GrantType = GrantTypes.RefreshToken }).Exchange();

        AssertInvalidGrant(result, "The user is no longer allowed to sign in.");
    }

    [Fact]
    public async Task ClientCredentialsGrant_UsesTheApplicationAsSubject()
    {
        var application = new object();
        _applications.Setup(a => a.FindByClientIdAsync("functions", It.IsAny<CancellationToken>())).ReturnsAsync(application);
        _applications.Setup(a => a.GetClientIdAsync(application, It.IsAny<CancellationToken>())).ReturnsAsync("functions");
        _applications.Setup(a => a.GetDisplayNameAsync(application, It.IsAny<CancellationToken>())).ReturnsAsync("Sigook Functions");

        IActionResult result = await Controller(new OpenIddictRequest { GrantType = GrantTypes.ClientCredentials, ClientId = "functions", Scope = "api1" }).Exchange();

        ClaimsPrincipal principal = Assert.IsType<SignInResult>(result).Principal;
        Assert.Equal("functions", principal.GetClaim(Claims.Subject));
        Assert.Equal("Sigook Functions", principal.GetClaim(Claims.Name));
        Assert.Equal(["api1"], principal.GetScopes().ToArray());
        Assert.All(principal.Claims, c => Assert.Equal([Destinations.AccessToken], c.GetDestinations().ToArray()));
    }

    [Fact]
    public async Task UnsupportedGrant_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => Controller(new OpenIddictRequest { GrantType = "urn:custom" }).Exchange());
    }

    [Fact]
    public async Task Userinfo_SingleRole_IsAString()
    {
        var caller = new ClaimsPrincipal(new ClaimsIdentity([new Claim(Claims.Subject, _user.Id.ToString())], "test"));

        var ok = Assert.IsType<OkObjectResult>(await Controller(new OpenIddictRequest(), caller).Userinfo());

        var claims = Assert.IsType<Dictionary<string, object>>(ok.Value);
        Assert.Equal(CovenantConstants.Role.Worker, claims[Claims.Role]);
        Assert.Equal(Email, claims[Claims.Email]);
        Assert.Equal(true, claims[Claims.EmailVerified]);
        Assert.Equal(Email, claims[IdentityClaims.Nickname]);
    }

    [Fact]
    public async Task Userinfo_SeveralRoles_IsAnArray()
    {
        _userManager.Setup(u => u.GetRolesAsync(_user)).ReturnsAsync([CovenantConstants.Role.Admin, CovenantConstants.Role.Sales]);
        var caller = new ClaimsPrincipal(new ClaimsIdentity([new Claim(Claims.Subject, _user.Id.ToString())], "test"));

        var ok = Assert.IsType<OkObjectResult>(await Controller(new OpenIddictRequest(), caller).Userinfo());

        var claims = Assert.IsType<Dictionary<string, object>>(ok.Value);
        Assert.Equal(new[] { CovenantConstants.Role.Admin, CovenantConstants.Role.Sales }, claims[Claims.Role]);
    }

    [Fact]
    public async Task Userinfo_DeactivatedUser_IsChallenged()
    {
        _sessionValidator.Setup(s => s.IsActive(_user)).ReturnsAsync(false);
        var caller = new ClaimsPrincipal(new ClaimsIdentity([new Claim(Claims.Subject, _user.Id.ToString())], "test"));

        var challenge = Assert.IsType<ChallengeResult>(await Controller(new OpenIddictRequest(), caller).Userinfo());

        Assert.Equal(Errors.InvalidToken, challenge.Properties!.Items[OpenIddictServerAspNetCoreConstants.Properties.Error]);
    }
}
