using Covenant.Common.Models.Notification;

namespace Covenant.Common.Interfaces;

public interface IPushNotificationSender
{
    Task Send(NotificationModel model);
}
