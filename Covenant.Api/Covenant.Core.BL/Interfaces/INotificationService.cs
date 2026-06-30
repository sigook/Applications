using Covenant.Common.Models.Notifications;

namespace Covenant.Core.BL.Interfaces;

public interface INotificationService
{
    Task<NotificationsModel> GetNotifications();
}
