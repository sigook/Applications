using System.Collections.Generic;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants.StandardScopes;

namespace Covenant.IdentityServer.Configuration
{
	public static class DevelopmentConfiguration
	{
		public static readonly Client Development = new Client
		{
			ClientId = "all2job",
			ClientName = "Sigook Development",
			RequireConsent = false,
			RequirePkce = true,
			AllowedGrantTypes = GrantTypes.Implicit,
			AllowAccessTokensViaBrowser = true,
			RedirectUris = new List<string>
			{
				"https://localhost:3001/callback",
				"https://localhost:3001/callback.html",
				"https://localhost:3001/home",
				"https://localhost:3001/silent-refresh",
				"https://staging.web.sigook.ca/callback",
				"https://staging.web.sigook.ca",
				"https://staging.web.sigook.ca/home",
				"https://staging.web.sigook.ca/silent-refresh",
			},
			PostLogoutRedirectUris = new List<string>
			{
				"https://localhost:3001",
				"https://staging.web.sigook.ca"
			},
			AllowedCorsOrigins = new List<string>
			{
				"https://localhost:3001",
				"https://staging.web.sigook.ca"
			},
			ClientUri = "https://staging.web.sigook.ca",
			AllowedScopes =
			{
				OpenId,
				Profile,
				"api1"
			}
		};
	}
}