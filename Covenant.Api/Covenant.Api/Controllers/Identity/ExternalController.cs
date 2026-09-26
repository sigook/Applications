using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Covenant.Api.Controllers.Identity;

[SecurityHeaders]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("External")]
public class ExternalController(
    UserManager<CovenantUser> userManager,
    SignInManager<CovenantUser> signInManager,
    IMicrosoft365AccountService accountService,
    ILogger<ExternalController> logger) : Controller
{
    private const string StaffEmailDomain = "@covenantgroupl.com";
    private const string ReturnUrlItem = "returnUrl";
    private const string SchemeItem = "scheme";

    [HttpGet("Challenge")]
    public IActionResult Challenge(string provider, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";
        if (!Url.IsLocalUrl(returnUrl))
        {
            logger.LogError("Invalid return URL detected: {ReturnUrl}", returnUrl);
            return BadRequest();
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback)),
            Items = { [ReturnUrlItem] = returnUrl, [SchemeItem] = provider }
        };
        return Challenge(properties, provider);
    }

    [HttpGet("Callback")]
    public async Task<IActionResult> Callback()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (result?.Succeeded != true)
        {
            logger.LogError("External authentication failed: {Error}", result?.Failure?.Message);
            return RedirectToAction(nameof(HomeController.InvalidUser), "Home");
        }

        var returnUrl = result.Properties.Items.TryGetValue(ReturnUrlItem, out var url) ? url : "~/";
        var scheme = result.Properties.Items.TryGetValue(SchemeItem, out var provider) ? provider : IdentityClaims.LocalIdentityProvider;

        var user = await FindStaffUser(result.Principal);
        if (user is null)
        {
            logger.LogError("Invalid user is trying to login via external provider. User: {User}", GetEmail(result.Principal) ?? "(unknown)");
            return RedirectToAction(nameof(HomeController.InvalidUser), "Home");
        }

        await signInManager.SignInWithClaimsAsync(user, isPersistent: false,
        [
            new Claim(IdentityClaims.Nickname, GetEmail(result.Principal)),
            new Claim(IdentityClaims.IdentityProvider, scheme)
        ]);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        if (!returnUrl.StartsWith("http", StringComparison.Ordinal) && !returnUrl.StartsWith('/') && !returnUrl.StartsWith("~/"))
        {
            return View("Redirect", new RedirectViewModel { RedirectUrl = returnUrl });
        }
        return Redirect(returnUrl);
    }

    private static string GetEmail(ClaimsPrincipal externalUser) =>
        externalUser?.FindFirst("preferred_username")?.Value
        ?? externalUser?.FindFirst("email")?.Value
        ?? externalUser?.FindFirst(ClaimTypes.Email)?.Value
        ?? externalUser?.FindFirst("name")?.Value;

    private async Task<CovenantUser> FindStaffUser(ClaimsPrincipal externalUser)
    {
        var email = GetEmail(externalUser);
        if (string.IsNullOrEmpty(email))
        {
            logger.LogWarning("External user has no email claim");
            return null;
        }

        if (!email.EndsWith(StaffEmailDomain, StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogWarning("External email domain validation failed: {Email}", email);
            return null;
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            logger.LogWarning("No Covenant user found for email: {Email}", email);
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            logger.LogWarning("Covenant user has no roles assigned: {Email}", email);
            return null;
        }

        var objectId = externalUser.FindFirst(IdentityClaims.MicrosoftObjectIdExternalClaim)?.Value;
        if (!string.IsNullOrEmpty(objectId))
        {
            await StoreMicrosoftObjectId(user, objectId);
            if (!await accountService.IsAccountEnabled(objectId))
            {
                logger.LogWarning("Microsoft 365 account is disabled, login rejected: {Email}", email);
                return null;
            }
        }
        return user;
    }

    private async Task StoreMicrosoftObjectId(CovenantUser user, string objectId)
    {
        var claims = await userManager.GetClaimsAsync(user);
        var existing = claims.FirstOrDefault(c => c.Type == IdentityClaims.MicrosoftObjectId);
        if (existing?.Value == objectId) return;

        var result = existing is null
            ? await userManager.AddClaimAsync(user, IdentityClaims.MicrosoftObject(objectId))
            : await userManager.ReplaceClaimAsync(user, existing, IdentityClaims.MicrosoftObject(objectId));
        if (!result.Succeeded)
        {
            logger.LogError("Could not store Microsoft object id for user {UserId}: {Errors}", user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
