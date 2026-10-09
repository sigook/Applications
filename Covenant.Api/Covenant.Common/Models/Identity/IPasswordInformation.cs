namespace Covenant.Common.Models.Identity;

public interface IPasswordInformation
{
    string Password { get; set; }
    string ConfirmPassword { get; set; }
}
