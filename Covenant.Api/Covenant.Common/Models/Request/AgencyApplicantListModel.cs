using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request;

public class AgencyApplicantListModel
{
    public Guid Id { get; set; }
    public Guid? CandidateId { get; set; }
    public Guid? WorkerProfileId { get; set; }
    public Guid? WorkerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Comments { get; set; }
    public RequestApplicantStatus Status { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RequestId { get; set; }
    public int RequestNumberId { get; set; }
    public string CompanyFullName { get; set; }
    public string JobTitle { get; set; }
    public DateTime? StartAt { get; set; }
    public bool IsAsap { get; set; }
    public bool IsDirectHiring { get; set; }
    public int ComplianceTotal { get; set; }
    public int ComplianceCompleted { get; set; }
    public int MandatoryPending { get; set; }
}
