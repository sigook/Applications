using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.WeeklyBoard;

public class WeeklyBoardRunnerModel
{
    public Guid RunnerId { get; set; }
    public Guid WorkerProfileId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public RunnerType Type { get; set; }
    public RunnerStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
}
