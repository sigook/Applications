namespace Covenant.Common.Models.Request
{
    public class RequestApplicantModel
    {
        public Guid? WorkerProfileId { get; set; }
        public Guid? CandidateId { get; set; }
        public string CreatedBy { get; set; }
        public string Comments { get; set; }
    }
}