using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Runners;

public class RunnerStatusHistoryModel
{
    public Guid Id { get; set; }
    public RunnerStatus? PreviousStatus { get; set; }
    public RunnerStatus NewStatus { get; set; }
    public string ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string Comments { get; set; }
}
