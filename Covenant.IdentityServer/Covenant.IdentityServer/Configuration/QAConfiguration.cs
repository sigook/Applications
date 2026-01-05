using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Covenant.IdentityServer.Configuration
{
	public static class QAConfiguration
	{
		private const string Domain = "sigook.online";
		public static readonly Client QA = new Client
		{
			ClientId = Domain,
			ClientName = Domain,
			RequireConsent = false,
			RequirePkce = true,
			AllowedGrantTypes = GrantTypes.Implicit,
			AllowAccessTokensViaBrowser = true,
			RedirectUris = new List<string>
			{
				$"https://www.{Domain}/callback.html",
				$"https://www.{Domain}/callback",
				$"https://www.{Domain}",
				$"https://www.{Domain}/home",
				$"https://www.{Domain}/silent-refresh",
				$"https://{Domain}/callback.html",
				$"https://{Domain}/callback",
				$"https://{Domain}",
				$"https://{Domain}/home",
				$"https://{Domain}/silent-refresh"
			},
			PostLogoutRedirectUris = new List<string>
			{
				$"https://{Domain}",
				$"https://www.{Domain}"
			},
			AllowedCorsOrigins = new List<string>
			{
				$"https://www.{Domain}",
				$"https://{Domain}"
			},
			ClientUri = $"https://www.{Domain}",
			AllowedScopes =
			{
				IdentityServerConstants.StandardScopes.OpenId,
				IdentityServerConstants.StandardScopes.Profile,
				"api1"
			}
		};
	}
}