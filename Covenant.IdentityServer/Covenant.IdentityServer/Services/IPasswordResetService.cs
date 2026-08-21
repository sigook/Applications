using Covenant.IdentityServer.Models;

namespace Covenant.IdentityServer.Services;

public interface IPasswordResetService
{
    Task RequestCodeAsync(string email);

    Task<PasswordResetResult> ResetPasswordAsync(string email, string code, string newPassword);
}
