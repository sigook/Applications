using Covenant.IdentityServer.Enums;
using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Models.Security;

public class CreateUserModel
{
    [Required, EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public UserType UserType { get; set; }
    [Required]
    public string Role { get; set; }
    public Guid? AgencyId { get; set; }
    public Guid? CompanyId { get; set; }
}
