using IdentityServer4;
using IdentityServer4.Models;

namespace Covenant.IdentityServer.Configuration
{
    public static class AccountingConfiguration
    {
        public static readonly Client Accounting = new Client
        {
            ClientId = "accounting.sigook.com",
            ClientName = "accounting.sigook.com",
            RequireConsent = false,
            RequirePkce = true,
            AllowedGrantTypes = GrantTypes.Code,
            AccessTokenLifetime = 86400,
            AllowAccessTokensViaBrowser = true,
            AllowOfflineAccess = true,
            RedirectUris = new List<string>
            {
                "https://accounting.sigook.com",
                "https://accounting.sigook.com/signin-oidc2",
                "https://accounting.sigook.com/signout-callback-oidc2",
                "https://localhost:9002",
                "https://localhost:9002/signin-oidc2",
                "https://localhost:9002/signout-callback-oidc2",
                "https://sigook-accounting-staging.azurewebsites.net",
                "https://sigook-accounting-staging.azurewebsites.net/signin-oidc2",
                "https://sigook-accounting-staging.azurewebsites.net/signout-callback-oidc2",
            },
            PostLogoutRedirectUris = new List<string>
            {
                "https://localhost:9002",
                "https://accounting.sigook.com",
                "https://sigook-accounting-staging.azurewebsites.net"
            },
            AllowedCorsOrigins = new List<string>
            {
                "https://localhost:9002",
                "https://accounting.sigook.com",
                "https://sigook-accounting-staging.azurewebsites.net"
            },
            ClientUri = "https://accounting.sigook.com",
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "api1",
                Config.Roles.Name
            },
            ClientSecrets = { new Secret("A51B26FB-8149-4024-9D08-4A3F833C2A39".Sha256()) },
        };
    }
}