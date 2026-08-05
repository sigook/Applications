using Covenant.Common.Enums;

namespace Covenant.Common.Models.Security;

public class CreateUserModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public UserType UserType { get; set; }
    public string Role { get; set; }
    public Guid? AgencyId { get; set; }
    public Guid? CompanyId { get; set; }
}
