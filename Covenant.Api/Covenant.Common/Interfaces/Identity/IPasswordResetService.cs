using Covenant.Common.Models.Identity;

namespace Covenant.Common.Interfaces.Identity;

public interface IPasswordResetService
{
    Task RequestCode(string email);
    Task RequestLink(string email);
    Task<PasswordResetResult> ResetPassword(string email, string code, string newPassword);
}
