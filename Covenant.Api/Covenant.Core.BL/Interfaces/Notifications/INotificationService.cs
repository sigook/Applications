using Covenant.Common.Models.Notification;

namespace Covenant.Core.BL.Interfaces.Notifications;

public interface INotificationService
{
    Task<NotificationsModel> GetNotifications();
}
