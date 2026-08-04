using Covenant.IdentityServer.Controllers.Account.Models;
using IdentityServer4.Models;

namespace Covenant.IdentityServer.Configuration
{
    public static class Config
    {
        public static IdentityResource Roles => new IdentityResource("roles", "Your role(s)", new[] { "role" });

        public static IEnumerable<ApiResource> GetApiResources()
        {
            var apiResource = new ApiResource("api1", "My API", new[] { "role" });
            foreach (var item in Constants.UserTypes)
            {
                apiResource.UserClaims.Add(item);
            }
            return new List<ApiResource> { apiResource };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var profile = new IdentityResources.OpenId();
            foreach (var userType in Constants.UserTypes)
            {
                profile.UserClaims.Add(userType);
            }
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                profile,
                Roles
            };
        }
    }
}