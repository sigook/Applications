namespace Covenant.Common.Models.Requests.Runners;

public class ChangeRunnersStatusResultModel
{
    public int Updated { get; set; }
    public List<SkippedRunnerModel> Skipped { get; set; } = [];
}
