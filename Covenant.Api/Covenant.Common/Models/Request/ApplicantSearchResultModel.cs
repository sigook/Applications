namespace Covenant.Common.Models.Request;

public class ApplicantSearchResultModel
{
    public Guid? WorkerProfileId { get; set; }
    public Guid? CandidateId { get; set; }
    public long NumberId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Type { get; set; }
    public bool ApprovedToWork { get; set; }
}
