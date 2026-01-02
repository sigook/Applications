using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants.StandardScopes;
using IdentityServer4.Models;

namespace Covenant.IdentityServer.Configuration
{
	public static class IosConfiguration
	{
		public static readonly Client Ios = new Client
		{
			ClientId = "ios",
			ClientName = "ios",
			RequireConsent = false,
			AllowedGrantTypes = GrantTypes.Code,
			AllowAccessTokensViaBrowser = true,
			RequireClientSecret = false,
			AllowOfflineAccess = true,
			FrontChannelLogoutSessionRequired = false,
			BackChannelLogoutSessionRequired = false,
			ClientSecrets = {new Secret("secret".Sha256()),},
			RedirectUris = new List<string> {"com.all2job:/oauth2redirect", "myiosappname:/oauth2redirect/example-provider"},
			PostLogoutRedirectUris = new List<string> {"com.all2job:/oauth2logout"},
			ClientUri = "https://www.sigook.com",
			AllowedScopes = {OpenId, Profile,"api1"},
		};
	}
}