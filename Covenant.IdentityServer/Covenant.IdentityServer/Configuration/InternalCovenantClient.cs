using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Covenant.IdentityServer.Configuration
{
	public static class InternalCovenantClient
	{
		public static readonly Client Client = new Client
		{
			ClientId = "4AC03D398D524457B35F631DC5ABB7D3",
			ClientName = "SIGOOK ADMIN",
			ClientSecrets = { new Secret("1B6003842F994524AB90C63A6808400A".Sha256()) },
			AllowedGrantTypes = GrantTypes.Code,
			RedirectUris =
			{
				"https://localhost:5002/signin-oidc",
				"https://localhost:9002/signin-oidc"
			},
			PostLogoutRedirectUris =
			{
				"https://localhost:5002/signout-callback-oidc",
				"https://localhost:9002/signout-callback-oidc"
			},
			RequirePkce = true,
			AllowOfflineAccess = true,
			AllowedScopes = new List<string>
			{
				IdentityServerConstants.StandardScopes.OpenId,
				IdentityServerConstants.StandardScopes.Profile,
				IdentityServerConstants.StandardScopes.Email,
				"api1",
				Config.Roles.Name
			}
		};
	}
}