using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants.StandardScopes;

namespace Covenant.IdentityServer.Configuration
{
	public static class AndroidConfiguration
	{
		public static readonly Client Android = new Client
		{
			ClientId = "android",
			ClientName = "Android",
			ClientSecrets = { new Secret("3ECF1EA2DAD04AE0A89002DB3C3421C8".Sha256())},
			RedirectUris =
			{
				"com.sigook:/oauth2redirect"
			},
			PostLogoutRedirectUris =
			{
				"com.sigook:/oauth2logout"
			},
			AllowedGrantTypes = GrantTypes.Code,
			AllowAccessTokensViaBrowser = true,
			RequireConsent = false,
			RequirePkce = true,
			AllowOfflineAccess = true,
			FrontChannelLogoutSessionRequired = false,
			BackChannelLogoutSessionRequired = false,
			RefreshTokenUsage = TokenUsage.ReUse,
			ClientUri = "https://www.sigook.com",
			AllowedScopes = {OpenId, Profile,"api1"},
			RequireClientSecret = false
		};
	}
}