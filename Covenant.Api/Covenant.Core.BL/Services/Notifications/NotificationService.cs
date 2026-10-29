using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Repositories.Notifications;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Notifications;

namespace Covenant.Core.BL.Services.Notifications;

public class NotificationService(
    INotificationRepository notificationRepository,
    IRequestRepository requestRepository,
    ICurrentUserService currentUserService) : INotificationService
{
    public async Task<NotificationsModel> GetNotifications()
    {
        if (currentUserService.IsSales() && !currentUserService.IsAdmin()) return new NotificationsModel();
        return new NotificationsModel
        {
            PendingReviewRequests = await requestRepository.GetPendingReviewRequests(currentUserService.GetAgencyId())
        };
    }

    public async Task UpdateUserNotification(Guid userId, UserNotificationUpdateModel model)
    {
        await notificationRepository.CreateUpdate(userId, model);
        await notificationRepository.SaveChangesAsync();
    }
}
