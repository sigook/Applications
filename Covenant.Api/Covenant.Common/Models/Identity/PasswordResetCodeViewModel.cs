namespace Covenant.Common.Models.Identity;

public class PasswordResetCodeViewModel
{
    public string Code { get; set; }
    public int ExpiresMinutes { get; set; }
}
