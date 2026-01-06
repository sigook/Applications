using System.Security.Claims;

namespace Covenant.IdentityServer.Controllers.Account.Models
{
    public class CovenantUserData
    {
        public CovenantUserData(string providerName, string providerUserId, string username, string subjectId, IList<Claim> claims)
        {
            ProviderName = providerName;
            ProviderUserId = providerUserId;
            Username = username;
            SubjectId = subjectId;
            Claims = claims;
        }

        public string ProviderName { get; }
        public string ProviderUserId { get; }
        public string Username { get; }
        public string SubjectId { get; }
        public IList<Claim> Claims { get; }
    }
}
