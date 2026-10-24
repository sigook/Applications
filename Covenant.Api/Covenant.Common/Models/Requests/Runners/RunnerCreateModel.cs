using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.Runners;

public class RunnerCreateModel
{
    public Guid WorkerProfileId { get; set; }
    public RunnerType Type { get; set; }
}
