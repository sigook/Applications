using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.IdentityServer.Controllers.Consent
{
    /// <summary>
    /// This controller processes the consent UI
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class ConsentController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly ILogger<ConsentController> _logger;

        public ConsentController(
            IIdentityServerInteractionService interaction,
            IEventService events,
            ILogger<ConsentController> logger)
        {
            _interaction = interaction;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            _logger.LogInformation("=== CONSENT SCREEN REQUESTED ===");
            _logger.LogInformation("ReturnUrl: {ReturnUrl}", returnUrl ?? "(null)");
            _logger.LogInformation("User: {UserId}", User.GetSubjectId() ?? "(null)");

            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                _logger.LogInformation("Consent Request Context:");
                _logger.LogInformation("  - ClientId: {ClientId}", request.Client?.ClientId ?? "(null)");
                _logger.LogInformation("  - ClientName: {ClientName}", request.Client?.ClientName ?? "(null)");
                _logger.LogInformation("  - Scopes Requested: {Scopes}", request.ValidatedResources?.RawScopeValues != null ? string.Join(", ", request.ValidatedResources.RawScopeValues) : "(none)");
                _logger.LogInformation("  - Identity Scopes: {IdentityScopes}", request.ValidatedResources?.Resources?.IdentityResources != null ? string.Join(", ", request.ValidatedResources.Resources.IdentityResources.Select(r => r.Name)) : "(none)");
                _logger.LogInformation("  - API Scopes: {ApiScopes}", request.ValidatedResources?.ParsedScopes != null ? string.Join(", ", request.ValidatedResources.ParsedScopes.Select(s => s.RawValue)) : "(none)");
                _logger.LogInformation("  - AllowRememberConsent: {AllowRememberConsent}", request.Client?.AllowRememberConsent ?? false);
            }
            else
            {
                _logger.LogWarning("No authorization context found for returnUrl: {ReturnUrl}", returnUrl ?? "(null)");
            }

            var vm = await BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                _logger.LogInformation("Consent screen built successfully");
                return View("Index", vm);
            }

            _logger.LogError("Failed to build consent view model - Showing error page");
            return View("Error");
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            _logger.LogInformation("=== CONSENT SUBMISSION ===");
            _logger.LogInformation("User: {UserId}", User.GetSubjectId() ?? "(null)");
            _logger.LogInformation("Button: {Button}", model?.Button ?? "(null)");
            _logger.LogInformation("ReturnUrl: {ReturnUrl}", model?.ReturnUrl ?? "(null)");
            _logger.LogInformation("RememberConsent: {RememberConsent}", model?.RememberConsent ?? false);
            _logger.LogInformation("Scopes Consented: {Scopes}", model?.ScopesConsented != null ? string.Join(", ", model.ScopesConsented) : "(none)");

            var result = await ProcessConsent(model);

            if (result.IsRedirect)
            {
                _logger.LogInformation("Consent processed successfully - Redirecting to: {RedirectUri}", result.RedirectUri);
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context?.IsNativeClient() == true)
                {
                    _logger.LogInformation("Native client detected - Using loading page");
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", result.RedirectUri);
                }

                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                _logger.LogWarning("Consent validation error: {ValidationError}", result.ValidationError);
                ModelState.AddModelError(string.Empty, result.ValidationError);
            }

            if (result.ShowView)
            {
                _logger.LogInformation("Redisplaying consent screen");
                return View("Index", result.ViewModel);
            }

            _logger.LogError("Consent processing failed - Showing error page");
            return View("Error");
        }

        /*****************************************/
        /* helper APIs for the ConsentController */
        /*****************************************/
        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            _logger.LogInformation("Processing consent decision");

            // validate return url is still valid
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null)
            {
                _logger.LogError("No authorization context found for ReturnUrl: {ReturnUrl}", model?.ReturnUrl ?? "(null)");
                return result;
            }

            _logger.LogInformation("Authorization context validated for ClientId: {ClientId}", request.Client.ClientId);

            ConsentResponse grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model?.Button == "no")
            {
                _logger.LogInformation("User denied consent for ClientId: {ClientId}", request.Client.ClientId);
                grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

                // emit event
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
            }
            // user clicked 'yes' - validate the data
            else if (model?.Button == "yes")
            {
                _logger.LogInformation("User granted consent for ClientId: {ClientId}", request.Client.ClientId);

                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    _logger.LogInformation("Scopes consented by user: {Scopes}", string.Join(", ", model.ScopesConsented));

                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
                        _logger.LogInformation("Offline access disabled - Filtered scopes: {Scopes}", string.Join(", ", scopes));
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };

                    _logger.LogInformation("Consent granted - RememberConsent: {RememberConsent}, Final Scopes: {Scopes}",
                        model.RememberConsent, string.Join(", ", grantedConsent.ScopesValuesConsented));

                    // emit event
                    await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
                }
                else
                {
                    _logger.LogWarning("User granted consent but no scopes selected");
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                _logger.LogWarning("Invalid consent button value: {Button}", model?.Button ?? "(null)");
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                _logger.LogInformation("Granting consent to IdentityServer");
                // communicate outcome of consent back to identityserver
                await _interaction.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
                _logger.LogInformation("Consent processing complete - Will redirect to: {RedirectUri}", result.RedirectUri);
            }
            else
            {
                _logger.LogInformation("Consent not granted - Rebuilding view model");
                // we need to redisplay the consent UI
                result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
            }

            return result;
        }

        private async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                return CreateConsentViewModel(model, returnUrl, request);
            }
            else
            {
                _logger.LogError("No consent request matching request: {0}", returnUrl);
            }

            return null;
        }

        private ConsentViewModel CreateConsentViewModel(
            ConsentInputModel model, string returnUrl,
            AuthorizationRequest request)
        {
            var vm = new ConsentViewModel
            {
                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
                Description = model?.Description,

                ReturnUrl = returnUrl,

                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                AllowRememberConsent = request.Client.AllowRememberConsent
            };

            vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeViewModel>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }
            if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
            }
            vm.ApiScopes = apiScopes;

            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Value = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}