namespace Covenant.Common.Models.Requests.Runners;

public class SkippedRunnerModel
{
    public Guid RunnerId { get; set; }
    public string Reason { get; set; }
}
