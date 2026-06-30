using Covenant.Common.Models.Request.Runners;

namespace Covenant.Common.Models.Notifications;

public class NotificationsModel
{
    public List<RunnerStartingTodayModel> WorkersToReview { get; set; } = [];
}
