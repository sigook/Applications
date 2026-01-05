using IdentityServer4;
using Microsoft.AspNetCore.Authentication;

namespace Covenant.IdentityServer.Configuration
{
    public static class Microsoft365OpenIdConnect
    {
        public const string Scheme = "oidc";
        public const string SchemeMicrosoft365Bearer = "Microsoft365Bearer";
        public const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        public static void AddMicrosoftAuthentication365(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration)
        {
            authenticationBuilder.AddOpenIdConnect(Scheme, "Microsoft 365", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.Authority = configuration.GetValue<string>("Microsoft365Authority");
                options.ClientId = configuration.GetValue<string>("Microsoft365ClientId");
                options.ClientSecret = configuration.GetValue<string>("Microsoft365ClientSecret");
                options.SaveTokens = true;
                options.ResponseType = "code";
                options.CallbackPath = "/signin-oidc";
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.RemoteSignOutPath = "/signout";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("https://graph.microsoft.com/User.Read");
                options.ClaimActions.Remove("ipaddr");
                options.ClaimActions.Remove("oid");
                options.ClaimActions.MapUniqueJsonKey("picture", "picture");
                options.ClaimActions.MapUniqueJsonKey("oid", "oid");
            });

            authenticationBuilder.AddJwtBearer(SchemeMicrosoft365Bearer, o =>
            {
                o.Authority = configuration.GetValue<string>("Microsoft365Authority");
                o.Audience = configuration.GetValue<string>("Microsoft365ClientAudience");
                o.TokenValidationParameters.ValidateIssuer = false;
            });
        }
    }
}