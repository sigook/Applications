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
            _logger.LogInformation("=== EXTERNAL AUTHENTICATION CHALLENGE ===");
            _logger.LogInformation("Provider: {Provider}", provider ?? "(null)");
            _logger.LogInformation("ReturnUrl: {ReturnUrl}", returnUrl ?? "(null)");

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            bool isLocalUrl = Url.IsLocalUrl(returnUrl);
            bool isValidReturnUrl = _interaction.IsValidReturnUrl(returnUrl);

            _logger.LogInformation("ReturnUrl validation - IsLocal: {IsLocal}, IsValidOIDC: {IsValid}",
                isLocalUrl, isValidReturnUrl);

            if (!isLocalUrl && !isValidReturnUrl)
            {
                _logger.LogError("Invalid return URL detected: {ReturnUrl}", returnUrl);
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

            _logger.LogInformation("Initiating external authentication challenge with provider: {Provider}", provider);
            return Challenge(props, provider);
        }

        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            _logger.LogInformation("=== EXTERNAL AUTHENTICATION CALLBACK ===");

            // read external identity from the temporary cookie
            AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                _logger.LogError("External authentication failed. Result: {Result}", result?.Failure?.Message ?? "Unknown error");
                throw new Exception("External authentication error");
            }

            _logger.LogInformation("External authentication succeeded");

            // Log scheme and return URL from properties
            string scheme = result.Properties.Items.ContainsKey("scheme") ? result.Properties.Items["scheme"] : "(unknown)";
            string returnUrl = result.Properties.Items.ContainsKey("returnUrl") ? result.Properties.Items["returnUrl"] : "~/";

            _logger.LogInformation("External Provider: {Provider}", scheme);
            _logger.LogInformation("ReturnUrl: {ReturnUrl}", returnUrl);

            // Log external claims
            if (result.Principal != null)
            {
                _logger.LogInformation("External user claims received:");
                foreach (var claim in result.Principal.Claims)
                {
                    // Don't log sensitive values, just the claim types
                    if (claim.Type == JwtClaimTypes.Email || claim.Type == JwtClaimTypes.PreferredUserName || claim.Type == JwtClaimTypes.Name)
                    {
                        _logger.LogInformation("  - {Type}: {Value}", claim.Type, claim.Value);
                    }
                    else
                    {
                        _logger.LogInformation("  - {Type}: [hidden]", claim.Type);
                    }
                }
            }

            CovenantUserData data = await IfValidUserGetCovenantAgency(result.Principal, scheme);
            if (data is null)
            {
                var userName = GetName(result.Principal)?.Value ?? "(unknown)";
                _logger.LogError("Invalid user is trying to login via external provider. User: {User}, Provider: {Provider}", userName, scheme);
                return RedirectToAction("InvalidUser", "Home");
            }

            _logger.LogInformation("Valid Covenant user found:");
            _logger.LogInformation("  - Username: {Username}", data.Username);
            _logger.LogInformation("  - SubjectId: {SubjectId}", data.SubjectId);
            _logger.LogInformation("  - ProviderName: {ProviderName}", data.ProviderName);
            _logger.LogInformation("  - Claims count: {ClaimsCount}", data.Claims.Count);

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for sign out from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            additionalLocalClaims.AddRange(data.Claims);

            _logger.LogInformation("Total claims to be added to local cookie: {ClaimCount}", additionalLocalClaims.Count);

            // issue authentication cookie for user
            await _events.RaiseAsync(new UserLoginSuccessEvent(data.ProviderName, data.ProviderUserId, data.SubjectId, data.Username));
            var isuser = new IdentityServerUser(data.SubjectId)
            {
                DisplayName = data.Username,
                IdentityProvider = data.ProviderName,
                AdditionalClaims = additionalLocalClaims
            };
            await HttpContext.SignInAsync(isuser, localSignInProps);

            _logger.LogInformation("Authentication cookie issued for external user");

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // check if external login is in the context of an OIDC request
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                _logger.LogInformation("External login in OIDC context - ClientId: {ClientId}", context.Client?.ClientId ?? "(null)");
                if (context.IsNativeClient())
                {
                    _logger.LogInformation("Native client detected - Using redirect view");
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return View("Redirect", new RedirectViewModel { RedirectUrl = returnUrl });
                }
            }

            _logger.LogInformation("Redirecting to: {ReturnUrl}", returnUrl);
            return Redirect(returnUrl);
        }

        private static Claim GetName(ClaimsPrincipal externalUser) =>
            externalUser?.FindFirst(JwtClaimTypes.PreferredUserName) ??
            externalUser?.FindFirst(JwtClaimTypes.Email) ??
            externalUser?.FindFirst(JwtClaimTypes.Name);

        private async Task<CovenantUserData> IfValidUserGetCovenantAgency(ClaimsPrincipal externalUser, string provider)
        {
            _logger.LogInformation("Validating external user for provider: {Provider}", provider);

            Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                throw new Exception("Unknown userid");

            _logger.LogInformation("External UserId: {UserId}", userIdClaim.Value);

            Claim externalEmail = GetName(externalUser);
            if (string.IsNullOrEmpty(externalEmail?.Value))
            {
                _logger.LogWarning("External user has no email claim");
                return null;
            }

            _logger.LogInformation("External Email: {Email}", externalEmail.Value);

            if (!externalEmail.Value.EndsWith("@covenantgroupl.com", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("External email domain validation failed. Email does not end with @covenantgroupl.com: {Email}", externalEmail.Value);
                return null;
            }

            _logger.LogInformation("Email domain validated - Searching for Covenant user");

            CovenantUser covenantUser = await _userManager.FindByEmailAsync(externalEmail.Value);

            if (covenantUser == null)
            {
                _logger.LogWarning("User not found by email: {Email}. Trying default Covenant email", externalEmail.Value);
                string defaultEmail = _configuration.GetValue<string>("CovenantEmail");
                covenantUser = await _userManager.FindByEmailAsync(defaultEmail);
                _logger.LogInformation("Using default Covenant account: {DefaultEmail}", defaultEmail);
            }
            else
            {
                _logger.LogInformation("Covenant user found: UserId={UserId}, UserName={UserName}", covenantUser.Id, covenantUser.UserName);
            }

            string providerUserId = userIdClaim.Value;
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.NickName, externalEmail.Value),
                new Claim(JwtClaimTypes.Role, "agency.personnel")
            };

            _logger.LogInformation("Base claims added: NickName={NickName}, Role=agency.personnel", externalEmail.Value);

            List<Claim> roles = externalUser.FindAll(Microsoft365OpenIdConnect.RoleClaimType)?
                .Select(s => new Claim(JwtClaimTypes.Role, s.Value)).ToList();
            if (roles != null && roles.Any())
            {
                _logger.LogInformation("Microsoft 365 roles found: {Roles}", string.Join(", ", roles.Select(r => r.Value)));
                claims.AddRange(roles);
            }
            else
            {
                _logger.LogInformation("No Microsoft 365 roles found");
            }

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