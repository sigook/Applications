using System.Collections.Generic;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants.StandardScopes;

namespace Covenant.IdentityServer.Configuration
{
	public static class ProductionSigookConfiguration
	{
		public static readonly Client Sigook = new Client
		{
			ClientId = "sigook.com",
			ClientName = "sigook.com",
			RequireConsent = false,
			RequirePkce = true,
			AllowedGrantTypes = GrantTypes.Implicit,
			AccessTokenLifetime = 86400,
			AllowAccessTokensViaBrowser = true,
			RedirectUris = new List<string>
			{
				"https://www.sigook.com/callback.html",
				"https://www.sigook.com/callback",
				"https://www.sigook.com",
				"https://www.sigook.com/home",
				"https://www.sigook.com/silent-refresh",
				"https://sigook.com/callback.html",
				"https://sigook.com/callback",
				"https://sigook.com",
				"https://sigook.com/home",
				"https://sigook.com/silent-refresh"
			},
			PostLogoutRedirectUris = new List<string>
			{
				"https://sigook.com",
				"https://www.sigook.com"
			},
			AllowedCorsOrigins = new List<string>
			{
				"https://www.sigook.com",
				"https://sigook.com"
			},
			ClientUri = "https://www.sigook.com",
			AllowedScopes =
			{
				OpenId,
				Profile,
				"api1"
			}
		};
	}
}