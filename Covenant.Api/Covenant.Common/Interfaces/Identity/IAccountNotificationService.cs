using Covenant.Common.Entities;

namespace Covenant.Common.Interfaces.Identity;

public interface IAccountNotificationService
{
    Task SendConfirmAccount(CovenantUser user, string message);
    Task SendConfirmAndSetPassword(CovenantUser user);
    Task ResendConfirmAccount(string email);
    Task SendPasswordResetCode(CovenantUser user, string code, int expiresMinutes);
}
