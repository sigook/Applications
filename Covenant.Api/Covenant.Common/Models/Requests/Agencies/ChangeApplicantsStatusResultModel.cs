namespace Covenant.Common.Models.Requests.Agencies;

public class ChangeApplicantsStatusResultModel
{
    public int Updated { get; set; }
    public List<SkippedApplicantModel> Skipped { get; set; } = [];
}
