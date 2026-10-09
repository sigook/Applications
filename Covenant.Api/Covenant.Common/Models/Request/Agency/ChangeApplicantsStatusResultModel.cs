namespace Covenant.Common.Models.Request.Agency;

public class ChangeApplicantsStatusResultModel
{
    public int Updated { get; set; }
    public List<SkippedApplicantModel> Skipped { get; set; } = [];
}
