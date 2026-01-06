using Microsoft.AspNetCore.Identity;

namespace Covenant.Common.Entities
{
    public class CovenantUser : IdentityUser<Guid>
    {
        public string Address { get; set; }
    }
}
