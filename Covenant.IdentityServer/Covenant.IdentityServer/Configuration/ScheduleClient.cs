using IdentityServer4.Models;
using System.Security.Claims;

namespace Covenant.IdentityServer.Configuration
{
    public static class ScheduleClient
    {
        public static readonly Client Schedule = new Client
        {
            ClientId = "schedule.service",
            ClientSecrets = { new Secret("EC26EC05-526D-4B12-A494-496464840AAA".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "api1" },
            RequireClientSecret = true,
            Claims = new List<ClientClaim> { new ClientClaim("service", "schedule") }
        };
    }
}