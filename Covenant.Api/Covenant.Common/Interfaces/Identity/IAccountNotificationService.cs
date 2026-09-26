using Covenant.Common.Entities;

namespace Covenant.Common.Interfaces.Identity;

public interface IAccountNotificationService
{
    Task SendConfirmAccount(CovenantUser user, string message);
    Task SendConfirmAndSetPassword(CovenantUser user);
    Task SendPasswordResetLink(CovenantUser user);
    Task SendPasswordResetCode(CovenantUser user, string code, int expiresMinutes);
}
