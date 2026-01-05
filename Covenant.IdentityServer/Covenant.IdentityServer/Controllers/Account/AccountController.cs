using Covenant.Common.Entities;
using Covenant.IdentityServer.Controllers.Account.Models;
using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Services;
using Covenant.IdentityServer.Views.Notifications;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Covenant.IdentityServer.Controllers.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public const string ConfirmEmailAddressName = "ConfirmEmailAddress";
        public const string CratepasswordName = "CratePassword";
        private readonly ILogger<AccountController> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly UserManager<CovenantUser> _userManager;
        private readonly SignInManager<CovenantUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IRazorViewToStringRenderer _renderer;
        private readonly CovenantContext _context;

        public AccountController(
            ILogger<AccountController> logger,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            UserManager<CovenantUser> userManager,
            SignInManager<CovenantUser> signInManager,
            IEmailService emailService,
            IConfiguration configuration,
            IRazorViewToStringRenderer renderer,
            CovenantContext context)
        {
            _logger = logger;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
            _renderer = renderer;
            _context = context;
        }

        [HttpGet]
        [Route("/Account/ConfirmEmailAddress", Name = ConfirmEmailAddressName)]
        public IActionResult ConfirmEmailAddress(string token, string id) => View(new ConfirmEmailAddressModel { Token = token, Id = id });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmailAddress(ConfirmEmailAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, Resources.Resources.InvalidRequest);
                return View(model);
            }

            CovenantUser user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, Resources.Resources.InvalidRequest);
                return View(model);
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (result.Succeeded) return RedirectToAction("Success", "Home");
            foreach (IdentityError error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        [HttpGet]
        [Route("/Account/CreatePassword", Name = CratepasswordName)]
        public IActionResult CreatePassword(string token, string id) => View(new CreatePasswordModel { Token = token, Id = id });

        [HttpPost]
        public async Task<IActionResult> CreatePassword(CreatePasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);
            CovenantUser user = await _userManager.FindByIdAsync(model.Id);
            if (user is null) return View(model);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
                return RedirectToAction("Success", "Home");
            }

            foreach (IdentityError error in result.Errors)
            {
                if (error.Code == "InvalidToken") model.InvalidToken = true;
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [Route("/Account/ResetPassword", Name = nameof(ResetPassword))]
        public IActionResult ResetPassword(string token, string id) => View(new CreatePasswordModel { Token = token, Id = id });

        /// <summary>
        /// When the user reset the password and the email was not confirmed,
        /// this is automatically confirmed because the user had problems understanding the process
        /// of reset and confirm account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(CreatePasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);
            CovenantUser user = await _userManager.FindByIdAsync(model.Id);
            if (user is null) return View(model);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded) _logger.LogError("Confirm account failed");
                }
                return RedirectToAction("Success", "Home");
            }
            foreach (IdentityError error in result.Errors)
            {
                if (error.Code == "InvalidToken") model.InvalidToken = true;
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RequestResetPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestResetPassword(RequestResetPasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);
            CovenantUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return RedirectToAction("Success", "Home", new { message = Resources.Resources.ResetPasswordSuccess });
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string link = Url.Link(nameof(ResetPassword), new { token, id = user.Id });
            string message = await _renderer.RenderViewToStringAsync("/Views/Notifications/ResetPassword.cshtml", new ResetPasswordViewModel { Url = link });

            bool result = await _emailService.SendEmail(user.Email, Resources.Resources.ResetPassword, message);
            if (result) return RedirectToAction("Success", "Home", new { message = Resources.Resources.ResetPasswordSuccess });

            ModelState.AddModelError("Email", "There was an error sending the request please try again later");
            return View(model);
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            LoginViewModel vm = await BuildLoginViewModelAsync(returnUrl);
            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }
            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }
                    return Redirect(model.ReturnUrl);
                }
                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }

            if (ModelState.IsValid)
            {
                CovenantUser existingUser = await _userManager.FindByEmailAsync(model.Username);
                if (existingUser == null) return await ReturnError(AccountOptions.InvalidCredentialsErrorMessage);
                if (await _context.InactiveUsers.AnyAsync(iu => iu.UserId == existingUser.Id)) return await ReturnError(AccountOptions.InactiveUser);
                if (!(await _signInManager.CheckPasswordSignInAsync(existingUser, model.Password, false)).Succeeded) return await ReturnError(AccountOptions.InvalidCredentialsErrorMessage);
                if (!await _userManager.IsEmailConfirmedAsync(existingUser)) return await ReturnError(AccountOptions.AccountNotConfirmedErrorMessage, false);
                SignInResult signInResult = await _signInManager.PasswordSignInAsync(existingUser, model.Password, model.RememberLogin, false);
                if (signInResult.Succeeded)
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(existingUser.Email, existingUser.Id.ToString(), existingUser.UserName));
                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    AuthenticationProperties props = null;
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    }
                    // issue authentication cookie with subject ID and username
                    var claims = await _userManager.GetClaimsAsync(existingUser);
                    var isuser = new IdentityServerUser(existingUser.Id.ToString())
                    {
                        DisplayName = existingUser.Email,
                        AdditionalClaims = claims
                    };
                    await HttpContext.SignInAsync(isuser, props);
                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }
                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }
                    if (Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl); // request for a local page                    
                    else if (string.IsNullOrEmpty(model.ReturnUrl)) return Redirect("~/");
                    else throw new Exception("invalid return URL"); // user might have clicked on a malicious link - should be logged
                }

                return await ReturnError(AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            LoginViewModel vm = await BuildLoginViewModelAsync(model);
            return View(vm);

            async Task<IActionResult> ReturnError(string msgError, bool isAccountConfirmed = true)
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, msgError));
                ModelState.AddModelError("", msgError);
                LoginViewModel vm2 = await BuildLoginViewModelAsync(model);
                vm2.IsAccountConfirmed = isAccountConfirmed;
                if (model.Username.EndsWith("@covenantgroupl.com", StringComparison.InvariantCultureIgnoreCase))
                {
                    const string errorCovenantEmployee = "It looks like you work for Covenant Group LTD. Please login using the corporate access link.";
                    ModelState.AddModelError("Account", errorCovenantEmployee);
                }
                return View(vm2);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationLink([FromQuery] string userName)
        {
            if (string.IsNullOrEmpty(userName)) return BadRequest();
            CovenantUser user = await _userManager.FindByEmailAsync(userName);
            if (user is null) return BadRequest();
            if (user.EmailConfirmed) return Ok();
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string action = Url.Action("ConfirmEmailAddress", "Account", new { token, id = user.Id });
            string link = string.Concat(_configuration.GetValue<string>("AppUrl"), action);
            string html = await _renderer.RenderViewToStringAsync("/Views/Notifications/ConfirmAccount.cshtml", new ConfirmAccountViewModel { Url = link, Message = Resources.Resources.ConfirmAccountGeneric });
            await _emailService.SendEmail(user.Email, Resources.Resources.ConfirmYourAccount, html);
            return Ok();
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            LogoutViewModel vm = await BuildLogoutViewModelAsync(logoutId);
            if (vm.ShowLogoutPrompt == false)
            {
                //Request.Headers.TryGetValue("HeaderReferer", out StringValues value);
                //ViewData["HeaderReferer"] = value;
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            LoggedOutViewModel vm = await BuildLoggedOutViewModelAsync(model.LogoutId);
            Request.Cookies.TryGetValue("ReturnUrl", out var postLogoutRedirectUri);
            if (User?.Identity.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();

                // delete local authentication cookie
                await HttpContext.SignOutAsync();
                await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await HttpContext.SignOutAsync(IdentityConstants.TwoFactorRememberMeScheme);
                await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
                await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
                //await HttpContext.SignOutAsync("oidc");

                foreach (string cookie in HttpContext.Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
                //idsrv.session
                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            /*//The sing out with the external provider was disabled because was not doing the redirect properly
			// check if we need to trigger sign-out at an upstream identity provider
			if (vm.TriggerExternalSignout)
			{
				// build a return URL so the upstream provider will redirect back
				// to us after the user has logged out. this allows us to then
				// complete our single sign-out processing.
				string url = Url.Action("Logout", new {logoutId = vm.LogoutId});
				var props = new AuthenticationProperties{RedirectUri = url};
				props.SetParameter("id_token_hint",await HttpContext.GetTokenAsync("id_token"));
				// this triggers a redirect to the external provider for sign-out
				
				await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
				return SignOut(props, vm.ExternalAuthenticationScheme);
			}*/

            if (!string.IsNullOrWhiteSpace(postLogoutRedirectUri))
            {
                vm.PostLogoutRedirectUri = postLogoutRedirectUri;
            }
            else
            {
                vm.PostLogoutRedirectUri = _configuration.GetValue<string>("WebClientUrl");
            }

            return View("LoggedOut", vm);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                    ExternalProviders = new ExternalProvider[]
                    {
                        new ExternalProvider
                        {
                            AuthenticationScheme = context.IdP
                        }
                    }
                };
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();
            var providers = schemes.Where(x => x.DisplayName != null)
               .Select(x => new ExternalProvider
               {
                   DisplayName = x.DisplayName ?? x.Name,
                   AuthenticationScheme = x.Name
               }).ToList();

            var allowLocal = true;
            string clientUri = string.Empty;
            if (context?.Client?.ClientId != null)
            {
                Client client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;
                    clientUri = client.ClientUri;
                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider =>
                            client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }
            var postLogoutRedirectUri = new Uri(context.RedirectUri);
            Response.Cookies.Append("ReturnUrl", $"{postLogoutRedirectUri.Scheme}://{postLogoutRedirectUri.Authority}");
            var model = new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray(),
                ClientUri = clientUri,
                ShowLinkHome = ShowLinkHome(context?.Client?.ClientId),
                ShowMicrosoft365Button = ShowLoginButtonToCovenantStaff(clientUri, context.RedirectUri)
            };
            return model;
        }

        private bool ShowLoginButtonToCovenantStaff(string clientUri, string redirectUri)
        {
            if ("https://accounting.sigook.com".Equals(clientUri)) return true;
            if (redirectUri.StartsWith("https://staging.web.sigook.ca")) return true;
            if (redirectUri.StartsWith("https://covenant.sigook.ca")) return true;
            if (redirectUri.StartsWith("http://localhost:3001")) return true;
            return false;
        }

        private static bool ShowLinkHome(string clientId)
        {
            switch (clientId)
            {
                case "all2job": return true;
                case "all2job.us": return true;
                case "all2job.com": return true;
                case "sigook.com": return true;
                default: return false;
            }
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            LoginViewModel vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };
            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            LogoutRequest context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            LogoutRequest logout = await _interaction.GetLogoutContextAsync(logoutId);
            if (logout is null) _logger.LogInformation("********* Logout is null");
            else
            {
                //redirect_uri
                foreach (string parameter in logout.Parameters)
                {
                    if (parameter == "redirect_uri")
                    {
                        logout.PostLogoutRedirectUri = logout.Parameters[parameter];
                    }
                }
            }

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}