using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.Runners;

public class ChangeRunnerStatusModel
{
    public RunnerStatus Status { get; set; }
    public string Comments { get; set; }
    public DateTime? StartDate { get; set; }
}
