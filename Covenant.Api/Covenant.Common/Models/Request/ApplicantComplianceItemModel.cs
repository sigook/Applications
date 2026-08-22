using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request;

public class ApplicantComplianceItemModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsMandatory { get; set; }
    public ComplianceDocumentTarget DocumentTarget { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CompletedBy { get; set; }
    public bool CanUpload { get; set; }
    public string ExistingFileUrl { get; set; }
}
