using Microsoft.AspNetCore.Identity;

namespace Covenant.Common.Entities.Identity;

public class CovenantUser : IdentityUser<Guid>
{
    public string Address { get; set; }
}
