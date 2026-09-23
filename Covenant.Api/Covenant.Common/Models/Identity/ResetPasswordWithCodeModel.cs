namespace Covenant.Common.Models.Identity;

public class ResetPasswordWithCodeModel
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}
