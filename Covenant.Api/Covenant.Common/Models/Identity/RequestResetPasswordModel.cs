using System.ComponentModel.DataAnnotations;

namespace Covenant.Common.Models.Identity;

public class RequestResetPasswordModel
{
    [Required, EmailAddress]
    public string Email { get; set; }
}
