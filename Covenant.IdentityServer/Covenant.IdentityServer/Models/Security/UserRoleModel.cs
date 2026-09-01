using Covenant.IdentityServer.Models;

namespace Covenant.IdentityServer.Models.Security;

public class UserRoleModel : IdModel
{
    public UserRoleModel()
    {
    }

    public UserRoleModel(Guid id, string role) : base(id) => Role = role;

    public string Role { get; set; }
}
