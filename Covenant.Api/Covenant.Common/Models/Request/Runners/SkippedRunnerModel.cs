namespace Covenant.Common.Models.Request.Runners;

public class SkippedRunnerModel
{
    public Guid RunnerId { get; set; }
    public string Reason { get; set; }
}
