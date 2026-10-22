using Covenant.Common.Models.Notifications;

namespace Covenant.Core.BL.Interfaces.Notifications;

public interface INotificationService
{
    Task<NotificationsModel> GetNotifications();
    Task UpdateUserNotification(Guid userId, UserNotificationUpdateModel model);
}
