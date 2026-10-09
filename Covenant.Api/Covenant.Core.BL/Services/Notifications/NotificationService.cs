using Covenant.Common.Models.Notification;
using Covenant.Core.BL.Interfaces.Notifications;

namespace Covenant.Core.BL.Services.Notifications;

public class NotificationService : INotificationService
{
    public Task<NotificationsModel> GetNotifications() => Task.FromResult(new NotificationsModel());
}
