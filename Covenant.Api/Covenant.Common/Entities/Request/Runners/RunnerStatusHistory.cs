using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Request.Runners;

public class RunnerStatusHistory
{
    private RunnerStatusHistory()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid RunnerId { get; private set; }
    public Runner Runner { get; private set; }
    public RunnerStatus? PreviousStatus { get; private set; }
    public RunnerStatus NewStatus { get; private set; }
    public string ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; } = DateTime.Now;
    public string Comments { get; private set; }

    public static RunnerStatusHistory Create(Guid runnerId, RunnerStatus? previousStatus, RunnerStatus newStatus, string changedBy, string comments) =>
        new()
        {
            RunnerId = runnerId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ChangedBy = changedBy,
            Comments = comments
        };
}
