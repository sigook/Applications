namespace Covenant.Common.Models.Notifications;

public class NotificationsModel
{
    public IEnumerable<PendingReviewRequestNotificationModel> PendingReviewRequests { get; set; } = [];
}
