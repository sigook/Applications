using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request
{
    public class RequestApplicantDetailModel : RequestApplicantModel
    {
        public Guid Id { get; set; }
        public RequestApplicantStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid? WorkerId { get; set; }
        public int ComplianceTotal { get; set; }
        public int ComplianceCompleted { get; set; }
        public int MandatoryPending { get; set; }
    }
}