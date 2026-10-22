using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.WeeklyBoard;

public class AddRunnerModel
{
    public Guid RequestId { get; set; }
    public DateTime WorkDate { get; set; }
    public Guid WorkerProfileId { get; set; }
    public RunnerType Type { get; set; }
}
