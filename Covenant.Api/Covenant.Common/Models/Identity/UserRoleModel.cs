namespace Covenant.Common.Models.Identity;

public class UserRoleModel : IdModel
{
    public UserRoleModel()
    {
    }

    public UserRoleModel(Guid id, string role) : base(id) => Role = role;

    public string Role { get; set; }
}
