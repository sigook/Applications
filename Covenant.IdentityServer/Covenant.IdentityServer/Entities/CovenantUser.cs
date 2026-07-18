using Microsoft.AspNetCore.Identity;

namespace Covenant.IdentityServer.Entities
{
    public class CovenantUser : IdentityUser<Guid>
    {
        public string Address { get; set; }
    }
}
