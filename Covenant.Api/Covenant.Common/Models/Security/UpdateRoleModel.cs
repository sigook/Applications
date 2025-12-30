using Covenant.Common.Enums;

namespace Covenant.Common.Models.Security;

public class UpdateRoleModel : IdModel
{
    public UserType UserType { get; set; }
}
