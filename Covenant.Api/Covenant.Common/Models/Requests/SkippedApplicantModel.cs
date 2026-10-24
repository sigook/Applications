namespace Covenant.Common.Models.Requests;

public class SkippedApplicantModel
{
    public Guid ApplicantId { get; set; }
    public string Reason { get; set; }
}
