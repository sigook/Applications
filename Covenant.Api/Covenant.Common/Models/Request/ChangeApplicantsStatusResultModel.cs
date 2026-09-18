namespace Covenant.Common.Models.Request;

public class ChangeApplicantsStatusResultModel
{
    public int Updated { get; set; }
    public List<SkippedApplicantModel> Skipped { get; set; } = [];
}
