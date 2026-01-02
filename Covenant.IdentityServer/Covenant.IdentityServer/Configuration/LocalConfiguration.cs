using System.Collections.Generic;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants.StandardScopes;

namespace Covenant.IdentityServer.Configuration
{
	public static class LocalConfiguration
	{
		public static readonly Client Local = new Client
		{
			ClientId = "sigook.local",
			ClientName = "sigook.local",
			RequireConsent = false,
			RequirePkce = true,
			AllowedGrantTypes = GrantTypes.Implicit,
			AccessTokenLifetime = 86400,
			AllowAccessTokensViaBrowser = true,
			RedirectUris = new List<string>
			{
				"https://www.sigook.local/callback.html",
				"https://www.sigook.local/callback",
				"https://www.sigook.local",
				"https://www.sigook.local/home",
				"https://www.sigook.local/silent-refresh",
				"https://sigook.local/callback.html",
				"https://sigook.local/callback",
				"https://sigook.local",
				"https://sigook.local/home",
				"https://sigook.local/silent-refresh"
			},
			PostLogoutRedirectUris = new List<string>
			{
				"https://sigook.local",
				"https://www.sigook.local"
			},
			AllowedCorsOrigins = new List<string>
			{
				"https://www.sigook.local",
				"https://sigook.local"
			},
			ClientUri = "https://www.sigook.local",
			AllowedScopes =
			{
				OpenId,
				Profile,
				"api1"
			}
		};
	}
}