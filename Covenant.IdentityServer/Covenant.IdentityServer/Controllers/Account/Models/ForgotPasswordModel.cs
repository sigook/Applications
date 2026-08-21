using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Controllers.Account.Models;

public class ForgotPasswordModel
{
    [Required] [EmailAddress] public string Email { get; set; }
}
