using Covenant.Common.Entities.Notification;
using Covenant.Common.Enums;
using Covenant.Common.Models.Notification;

namespace Covenant.Common.Repositories.Notification
{
    public interface INotificationRepository
    {
        Task<UserNotificationType> Get(Guid userId, int typeId);
        Task<List<UserNotificationListModel>> Get(Guid userId, IEnumerable<NotificationTarget> targetAllow);
        Task Create(UserNotificationType entity);
        Task Update(UserNotificationType entity);
        Task CreateUpdate(Guid userId, UserNotificationUpdateModel model);
        Task SaveChangesAsync();
    }
}