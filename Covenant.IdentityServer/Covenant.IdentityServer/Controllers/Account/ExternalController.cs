using Covenant.Common.Entities;
using Covenant.IdentityServer.Configuration;
using Covenant.IdentityServer.Controllers.Account.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Covenant.IdentityServer.Controllers.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly UserManager<CovenantUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalController> _logger;

        public ExternalController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            UserManager<CovenantUser> userManager,
            IConfiguration configuration,
            ILogger<ExternalController> logger)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                }
            };

            return Challenge(props, provider);
        }

        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            CovenantUserData data = await IfValidUserGetCovenantAgency(result.Principal, result.Properties.Items["scheme"]);
            if (data is null)
            {
                _logger.LogError("Invalid user is trying to login {User}", GetName(result.Principal));
                return RedirectToAction("InvalidUser", "Home");
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for sign out from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            additionalLocalClaims.AddRange(data.Claims);

            // issue authentication cookie for user
            await _events.RaiseAsync(new UserLoginSuccessEvent(data.ProviderName, data.ProviderUserId, data.SubjectId, data.Username));
            var isuser = new IdentityServerUser(data.SubjectId)
            {
                DisplayName = data.Username,
                IdentityProvider = data.ProviderName,
                AdditionalClaims = additionalLocalClaims
            };
            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            string returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return View("Redirect", new RedirectViewModel { RedirectUrl = returnUrl });
                }
            }

            return Redirect(returnUrl);
        }

        private static Claim GetName(ClaimsPrincipal externalUser) =>
            externalUser?.FindFirst(JwtClaimTypes.PreferredUserName) ??
            externalUser?.FindFirst(JwtClaimTypes.Email) ??
            externalUser?.FindFirst(JwtClaimTypes.Name);

        private async Task<CovenantUserData> IfValidUserGetCovenantAgency(ClaimsPrincipal externalUser, string provider)
        {
            Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                throw new Exception("Unknown userid");

            Claim externalEmail = GetName(externalUser);
            if (string.IsNullOrEmpty(externalEmail?.Value)) return null;
            if (!externalEmail.Value.EndsWith("@covenantgroupl.com", StringComparison.InvariantCultureIgnoreCase)) return null;

            CovenantUser covenantUser =
                await _userManager.FindByEmailAsync(externalEmail.Value) ??
                await _userManager.FindByEmailAsync(_configuration.GetValue<string>("CovenantEmail"));

            string providerUserId = userIdClaim.Value;
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.NickName, externalEmail.Value),
                new Claim(JwtClaimTypes.Role, "agency.personnel")
            };

            List<Claim> roles = externalUser.FindAll(Microsoft365OpenIdConnect.RoleClaimType)?
                .Select(s => new Claim(JwtClaimTypes.Role, s.Value)).ToList();
            if (roles != null && roles.Any()) claims.AddRange(roles);

            return new CovenantUserData(provider, providerUserId, covenantUser.UserName, covenantUser.Id.ToString(), claims);
        }

        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            Claim sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            string idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }
    }
}