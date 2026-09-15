using Covenant.Common.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Covenant.Api.Configuration;

public static class Microsoft365OpenIdConnect
{
    public const string Scheme = "oidc";
    public const string DisplayName = "Microsoft 365";

    public static AuthenticationBuilder AddMicrosoftAuthentication365(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
    {
        var tenantId = configuration[$"{nameof(Microsoft365Configuration)}:{nameof(Microsoft365Configuration.TenantId)}"];
        var identity = configuration.GetSection(IdentityConfiguration.SectionName).Get<IdentityConfiguration>() ?? new IdentityConfiguration();
        return authenticationBuilder.AddOpenIdConnect(Scheme, DisplayName, options =>
        {
            options.SignInScheme = IdentityConstants.ExternalScheme;
            options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            options.ClientId = identity.Microsoft365ClientId;
            options.ClientSecret = identity.Microsoft365ClientSecret;
            options.SaveTokens = true;
            options.ResponseType = "code";
            options.CallbackPath = "/signin-oidc";
            options.SignedOutCallbackPath = "/signout-callback-oidc";
            options.RemoteSignOutPath = "/signout";
            options.GetClaimsFromUserInfoEndpoint = true;
            options.MapInboundClaims = false;
            options.Scope.Add("https://graph.microsoft.com/User.Read");
            options.ClaimActions.Remove("ipaddr");
            options.ClaimActions.Remove("oid");
            options.ClaimActions.MapUniqueJsonKey("picture", "picture");
            options.ClaimActions.MapUniqueJsonKey("oid", "oid");
        });
    }
}
