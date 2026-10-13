using Covenant.Common.Entities.Notification;
using Covenant.Common.Enums;
using Covenant.Common.Models.Notification;

namespace Covenant.Common.Repositories.Notifications;

public interface INotificationRepository : IBaseRepository
{
    Task<UserNotificationType> Get(Guid userId, int typeId);
    Task<List<UserNotificationListModel>> Get(Guid userId, IEnumerable<NotificationTarget> targetAllow);
    Task CreateUpdate(Guid userId, UserNotificationUpdateModel model);
}
