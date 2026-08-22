using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Controllers.Account.Models;

public class ResetPasswordWithCodeModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] [RegularExpression("^[0-9]{6}$")] public string Code { get; set; }

    [Required] [MinLength(6)] public string NewPassword { get; set; }
}
