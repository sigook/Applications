using Covenant.Common.Models.Notifications;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class NotificationService(IRunnerService runnerService) : INotificationService
{
    public async Task<NotificationsModel> GetNotifications() =>
        new()
        {
            WorkersToReview = await runnerService.GetRunnersStartingToday()
        };
}
