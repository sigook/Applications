using Covenant.Api.Authorization;
using Covenant.Api.Configuration;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Covenant.Common.Repositories.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OpenIddict.Abstractions;

namespace Covenant.Api.Controllers.Identity;

[SecurityHeaders]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("Account")]
public class AccountController(
    UserManager<CovenantUser> userManager,
    SignInManager<CovenantUser> signInManager,
    IIdentityRepository identityRepository,
    IAccountNotificationService notifications,
    IPasswordResetService passwordResetService,
    IOpenIddictApplicationManager applicationManager,
    IConfiguration configuration,
    ILogger<AccountController> logger) : Controller
{
    private static readonly TimeSpan RememberMeDuration = TimeSpan.FromDays(30);
    private static readonly string[] HomeLinkClients = ["all2job", "all2job.us", "all2job.com", "sigook.com"];
    private static readonly string[] StaffRedirectPrefixes = ["https://staging.web.sigook.ca", "https://covenant.sigook.ca", "http://localhost:3001"];

    [HttpGet("Login")]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var model = await BuildLoginViewModel(returnUrl);
        if (!string.IsNullOrEmpty(model.ExternalLoginScheme))
        {
            return RedirectToAction(nameof(ExternalController.Challenge), "External", new { provider = model.ExternalLoginScheme, returnUrl });
        }
        return View(model);
    }

    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel model)
    {
        if (!ModelState.IsValid) return View(await BuildLoginViewModel(model));

        var user = await userManager.FindByEmailAsync(model.Username);
        if (user is null)
        {
            logger.LogWarning("Login failed: user not found for {Email}", model.Username);
            return await LoginError(model, AccountMessages.InvalidCredentials);
        }

        if (await identityRepository.IsInactive(user.Id))
        {
            logger.LogWarning("Login failed: user is inactive. UserId={UserId}", user.Id);
            return await LoginError(model, AccountMessages.InactiveUser);
        }

        var passwordCheck = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!passwordCheck.Succeeded)
        {
            logger.LogWarning("Login failed: invalid password for {Email}", model.Username);
            return await LoginError(model, AccountMessages.InvalidCredentials);
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            logger.LogWarning("Login failed: email not confirmed for {Email}", model.Username);
            return await LoginError(model, AccountMessages.AccountNotConfirmed, isAccountConfirmed: false);
        }

        AuthenticationProperties properties = null;
        if (model.RememberLogin)
        {
            properties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.Add(RememberMeDuration) };
        }
        await signInManager.SignInWithClaimsAsync(user, properties, [new(IdentityClaims.IdentityProvider, IdentityClaims.LocalIdentityProvider)]);
        logger.LogInformation("Login succeeded for UserId={UserId}", user.Id);

        if (Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);
        return Redirect("~/");
    }

    [HttpGet("Logout"), HttpPost("Logout")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Logout()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
        return View("LoggedOut", new LoggedOutViewModel { PostLogoutRedirectUri = configuration.GetWebClientUrl() });
    }

    [HttpGet("ConfirmEmailAddress")]
    public IActionResult ConfirmEmailAddress(string token, string id) => View(new ConfirmEmailAddressModel { Token = token, Id = id });

    [HttpPost("ConfirmEmailAddress")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEmailAddress(ConfirmEmailAddressModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, AccountMessages.InvalidRequest);
            return View(model);
        }

        var user = await userManager.FindByIdAsync(model.Id);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, AccountMessages.InvalidRequest);
            return View(model);
        }

        var result = await userManager.ConfirmEmailAsync(user, model.Token);
        if (result.Succeeded) return RedirectToAction(nameof(HomeController.Success), "Home");
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    [HttpGet("CreatePassword")]
    public IActionResult CreatePassword(string token, string id) => View(new CreatePasswordModel { Token = token, Id = id });

    [HttpPost("CreatePassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePassword(CreatePasswordModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await userManager.FindByIdAsync(model.Id);
        if (user is null) return View(model);

        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, token);
            return RedirectToAction(nameof(HomeController.Success), "Home");
        }

        AddPasswordErrors(model, result);
        return View(model);
    }

    [HttpGet("ResetPassword")]
    public IActionResult ResetPassword(string token, string id) => View(new CreatePasswordModel { Token = token, Id = id });

    [HttpPost("ResetPassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(CreatePasswordModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await userManager.FindByIdAsync(model.Id);
        if (user is null) return View(model);

        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                var update = await userManager.UpdateAsync(user);
                if (!update.Succeeded) logger.LogError("Confirm account failed. UserId={UserId}", user.Id);
            }
            return RedirectToAction(nameof(HomeController.Success), "Home");
        }

        AddPasswordErrors(model, result);
        return View(model);
    }

    [HttpGet("RequestResetPassword")]
    public IActionResult RequestResetPassword() => View();

    [HttpPost("RequestResetPassword")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting(RateLimitingConfiguration.PasswordResetPolicy)]
    public async Task<IActionResult> RequestResetPassword(RequestResetPasswordModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await passwordResetService.RequestLink(model.Email);
        return RedirectToAction(nameof(HomeController.Success), "Home", new { message = AccountMessages.ResetPasswordSuccess });
    }

    [HttpPost("ResendConfirmationLink")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ResendConfirmationLink([FromQuery] string userName)
    {
        if (string.IsNullOrEmpty(userName)) return BadRequest();
        var user = await userManager.FindByEmailAsync(userName);
        if (user is null) return BadRequest();
        if (user.EmailConfirmed) return Ok();
        await notifications.SendConfirmAccount(user, AccountMessages.ConfirmAccountGeneric);
        return Ok();
    }

    private async Task<IActionResult> LoginError(LoginInputModel model, string message, bool isAccountConfirmed = true)
    {
        ModelState.AddModelError(string.Empty, message);
        var viewModel = await BuildLoginViewModel(model);
        viewModel.IsAccountConfirmed = isAccountConfirmed;
        return View(viewModel);
    }

    private async Task<LoginViewModel> BuildLoginViewModel(LoginInputModel model)
    {
        var viewModel = await BuildLoginViewModel(model.ReturnUrl);
        viewModel.Username = model.Username;
        viewModel.RememberLogin = model.RememberLogin;
        return viewModel;
    }

    private async Task<LoginViewModel> BuildLoginViewModel(string returnUrl)
    {
        var model = new LoginViewModel { ReturnUrl = returnUrl };
        var request = ParseAuthorizationRequest(returnUrl);
        if (request is null) return model;

        if (request.GetAcrValues().Contains($"idp:{Microsoft365OpenIdConnect.Scheme}"))
        {
            model.ExternalLoginScheme = Microsoft365OpenIdConnect.Scheme;
        }
        model.Username = request.LoginHint;

        var application = string.IsNullOrEmpty(request.ClientId) ? null : await applicationManager.FindByClientIdAsync(request.ClientId);
        if (application is not null)
        {
            var properties = await applicationManager.GetPropertiesAsync(application);
            model.ClientUri = properties.TryGetValue("client_uri", out var clientUri) ? clientUri.GetString() : string.Empty;
            model.ShowLinkHome = HomeLinkClients.Contains(request.ClientId);
        }
        model.ShowMicrosoft365Button = ShowMicrosoft365Button(model.ClientUri, request.RedirectUri);
        return model;
    }

    private static OpenIddictRequest ParseAuthorizationRequest(string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || !Uri.TryCreate(returnUrl, UriKind.Relative, out _)) return null;
        var queryIndex = returnUrl.IndexOf('?');
        if (queryIndex < 0) return null;
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(returnUrl[queryIndex..]);
        return new OpenIddictRequest(query.ToDictionary(p => p.Key, p => (string)p.Value));
    }

    private static bool ShowMicrosoft365Button(string clientUri, string redirectUri)
    {
        if ("https://accounting.sigook.com".Equals(clientUri)) return true;
        if (string.IsNullOrEmpty(redirectUri)) return false;
        return StaffRedirectPrefixes.Any(prefix => redirectUri.StartsWith(prefix, StringComparison.Ordinal));
    }

    private void AddPasswordErrors(CreatePasswordModel model, IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            if (error.Code == "InvalidToken") model.InvalidToken = true;
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
