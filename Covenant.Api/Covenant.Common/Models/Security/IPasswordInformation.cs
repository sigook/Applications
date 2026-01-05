namespace Covenant.Common.Models.Security
{
    public interface IPasswordInformation
    {
        string Password { get; set; }
        string ConfirmPassword { get; set; }
    }
}
