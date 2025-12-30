using Covenant.Common.Models.Notification;

namespace Covenant.Common.Interfaces
{
    public interface IPushNotifications
    {
        Task SendNotification(NotificationModel model);
    }
}