using Covenant.Common.Models.Notifications;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class NotificationService : INotificationService
{
    public Task<NotificationsModel> GetNotifications() => Task.FromResult(new NotificationsModel());
}
