using Covenant.IdentityServer.Models;

namespace Covenant.IdentityServer.Models.Security;

public class UpdateRoleModel : IdModel
{
    public string Role { get; set; }
}
