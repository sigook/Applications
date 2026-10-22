using Covenant.Common.Models.Notifications;
using Covenant.Common.Repositories.Notifications;
using Covenant.Core.BL.Interfaces.Notifications;

namespace Covenant.Core.BL.Services.Notifications;

public class NotificationService(INotificationRepository notificationRepository) : INotificationService
{
    public Task<NotificationsModel> GetNotifications() => Task.FromResult(new NotificationsModel());

    public async Task UpdateUserNotification(Guid userId, UserNotificationUpdateModel model)
    {
        await notificationRepository.CreateUpdate(userId, model);
        await notificationRepository.SaveChangesAsync();
    }
}
