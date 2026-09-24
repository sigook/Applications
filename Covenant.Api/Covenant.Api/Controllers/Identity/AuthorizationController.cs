using Covenant.Api.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Repositories.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Collections.Immutable;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Covenant.Api.Controllers.Identity;

[ApiExplorerSettings(IgnoreApi = true)]
public class AuthorizationController(
    UserManager<CovenantUser> userManager,
    SignInManager<CovenantUser> signInManager,
    IIdentityRepository identityRepository,
    IUserSessionValidator sessionValidator,
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictScopeManager scopeManager,
    ILogger<AuthorizationController> logger) : Controller
{
    private const string ExternalProviderAcrValue = "idp:" + Microsoft365OpenIdConnect.Scheme;
    private const string InvalidCredentials = "invalid_credentials";
    private const string InactiveUser = "inactive_user";
    private const string EmailNotConfirmed = "email_not_confirmed";
    private const string LockedOut = "locked_out";

    [HttpGet("~/connect/authorize"), HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

        if (result?.Succeeded != true)
        {
            if (request.HasPromptValue(PromptValues.None))
            {
                return Forbid(
                    new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                    }),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var returnUrl = Request.PathBase + Request.Path + Request.QueryString;
            if (request.GetAcrValues().Contains(ExternalProviderAcrValue))
            {
                return RedirectToAction(nameof(ExternalController.Challenge), "External", new { provider = Microsoft365OpenIdConnect.Scheme, returnUrl });
            }
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, IdentityConstants.ApplicationScheme);
        }

        var user = await userManager.GetUserAsync(result.Principal);
        if (user is null || !await signInManager.CanSignInAsync(user) || !await sessionValidator.IsActive(user))
        {
            await signInManager.SignOutAsync();
            return Forbid(
                new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                }),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var principal = await CreatePrincipal(user, request.GetScopes(),
            result.Principal.FindFirst(IdentityClaims.IdentityProvider)?.Value,
            result.Principal.FindFirst(IdentityClaims.Nickname)?.Value);
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsPasswordGrantType())
        {
            return await ExchangePassword(request);
        }

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var user = await userManager.FindByIdAsync(result.Principal?.GetClaim(Claims.Subject) ?? string.Empty);
            if (user is null)
            {
                return InvalidGrant("The token is no longer valid.");
            }
            if (!await signInManager.CanSignInAsync(user) || !await sessionValidator.IsActive(user))
            {
                return InvalidGrant("The user is no longer allowed to sign in.");
            }
            var principal = await CreatePrincipal(user, result.Principal.GetScopes(),
                result.Principal.GetClaim(IdentityClaims.IdentityProvider),
                result.Principal.GetClaim(IdentityClaims.Nickname));
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsClientCredentialsGrantType())
        {
            var application = await applicationManager.FindByClientIdAsync(request.ClientId)
                ?? throw new InvalidOperationException("The application details cannot be found.");
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);
            identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application))
                .SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));
            identity.SetScopes(request.GetScopes());
            identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
            identity.SetDestinations(_ => [Destinations.AccessToken]);
            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Userinfo()
    {
        var user = await userManager.FindByIdAsync(User.GetClaim(Claims.Subject) ?? string.Empty);
        if (user is null || !await sessionValidator.IsActive(user))
        {
            return Challenge(
                new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is bound to an account that no longer exists."
                }),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var roles = await userManager.GetRolesAsync(user);
        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [Claims.Subject] = user.Id.ToString(),
            [Claims.Name] = user.Email,
            [Claims.PreferredUsername] = user.Email,
            [Claims.Email] = user.Email,
            [Claims.EmailVerified] = user.EmailConfirmed,
            [IdentityClaims.Nickname] = User.GetClaim(IdentityClaims.Nickname) ?? user.Email
        };
        if (roles.Count == 1) claims[Claims.Role] = roles[0];
        else if (roles.Count > 1) claims[Claims.Role] = roles.ToArray();

        foreach (var claim in await userManager.GetClaimsAsync(user))
        {
            if (claim.Type is CovenantConstants.AgencyId or CovenantConstants.CompanyId) claims[claim.Type] = claim.Value;
        }
        return Ok(claims);
    }

    [HttpGet("~/connect/endsession"), HttpPost("~/connect/endsession")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> EndSession()
    {
        await signInManager.SignOutAsync();
        return SignOut(new AuthenticationProperties { RedirectUri = "/" }, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> ExchangePassword(OpenIddictRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Username ?? string.Empty);
        if (user is null)
        {
            logger.LogWarning("Native login failed: user not found for {Email}", request.Username);
            return InvalidGrant(InvalidCredentials);
        }

        if (await identityRepository.IsInactive(user.Id))
        {
            logger.LogWarning("Native login failed: user is inactive. UserId={UserId}", user.Id);
            return InvalidGrant(InactiveUser);
        }

        var passwordCheck = await signInManager.CheckPasswordSignInAsync(user, request.Password ?? string.Empty, lockoutOnFailure: true);
        if (passwordCheck.IsLockedOut)
        {
            logger.LogWarning("Native login failed: user is locked out. UserId={UserId}", user.Id);
            return InvalidGrant(LockedOut);
        }
        if (!passwordCheck.Succeeded)
        {
            logger.LogWarning("Native login failed: invalid password for {Email}", request.Username);
            return InvalidGrant(InvalidCredentials);
        }
        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            logger.LogWarning("Native login failed: email not confirmed for {Email}", request.Username);
            return InvalidGrant(EmailNotConfirmed);
        }

        logger.LogInformation("Native login succeeded for UserId={UserId}", user.Id);
        var principal = await CreatePrincipal(user, request.GetScopes(), IdentityClaims.LocalIdentityProvider, null);
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private IActionResult InvalidGrant(string description) =>
        Forbid(
            new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description
            }),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    private async Task<ClaimsPrincipal> CreatePrincipal(CovenantUser user, ImmutableArray<string> scopes, string identityProvider, string nickname)
    {
        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);
        identity.SetClaim(Claims.Subject, user.Id.ToString())
            .SetClaim(Claims.Name, user.Email)
            .SetClaim(Claims.PreferredUsername, user.Email)
            .SetClaim(Claims.Email, user.Email)
            .SetClaim(Claims.EmailVerified, user.EmailConfirmed)
            .SetClaim(IdentityClaims.Nickname, nickname ?? user.Email)
            .SetClaim(IdentityClaims.IdentityProvider, identityProvider ?? IdentityClaims.LocalIdentityProvider)
            .SetClaims(Claims.Role, [.. await userManager.GetRolesAsync(user)]);

        foreach (var claim in await userManager.GetClaimsAsync(user))
        {
            if (claim.Type is CovenantConstants.AgencyId or CovenantConstants.CompanyId) identity.SetClaim(claim.Type, claim.Value);
        }

        identity.SetScopes(scopes);
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
        identity.SetDestinations(GetDestinations);
        return new ClaimsPrincipal(identity);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Subject:
            case IdentityClaims.IdentityProvider:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;
                if (claim.Subject.HasScope(Scopes.Roles)) yield return Destinations.IdentityToken;
                yield break;

            case Claims.Name:
            case Claims.PreferredUsername:
            case IdentityClaims.Nickname:
            case CovenantConstants.AgencyId:
            case CovenantConstants.CompanyId:
                yield return Destinations.AccessToken;
                if (claim.Subject.HasScope(Scopes.Profile)) yield return Destinations.IdentityToken;
                yield break;

            case Claims.Email:
            case Claims.EmailVerified:
                yield return Destinations.AccessToken;
                if (claim.Subject.HasScope(Scopes.Email)) yield return Destinations.IdentityToken;
                yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
