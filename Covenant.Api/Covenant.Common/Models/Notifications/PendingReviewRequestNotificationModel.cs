namespace Covenant.Common.Models.Notifications;

public class PendingReviewRequestNotificationModel
{
    public Guid RequestId { get; set; }
    public int NumberId { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public DateTime CreatedAt { get; set; }
}
