namespace Covenant.Common.Models.Request;

public class SkippedApplicantModel
{
    public Guid ApplicantId { get; set; }
    public string Reason { get; set; }
}
