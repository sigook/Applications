using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Runners;

public class RunnerCreateModel
{
    public Guid? WorkerProfileId { get; set; }
    public Guid? CandidateId { get; set; }
    public RunnerType Type { get; set; }
}
