using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests;

public class RequestComplianceItemModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public bool IsMandatory { get; set; }
    public ComplianceDocumentTarget DocumentTarget { get; set; }
}
