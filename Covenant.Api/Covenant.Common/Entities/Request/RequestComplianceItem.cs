using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Request;

public class RequestComplianceItem
{
    private RequestComplianceItem()
    {
    }

    public Guid Id { get; internal set; } = Guid.NewGuid();
    public string Name { get; internal set; }
    public bool IsMandatory { get; internal set; }
    public ComplianceDocumentTarget DocumentTarget { get; internal set; }
    public Request Request { get; private set; }
    public Guid RequestId { get; private set; }

    public static Result<RequestComplianceItem> Create(Guid requestId, string name, bool isMandatory, ComplianceDocumentTarget documentTarget)
    {
        return string.IsNullOrWhiteSpace(name)
            ? Result.Fail<RequestComplianceItem>(ValidationMessages.RequiredMsg(nameof(Name)))
            : Result.Ok(new RequestComplianceItem { RequestId = requestId, Name = name.Trim(), IsMandatory = isMandatory, DocumentTarget = documentTarget });
    }

    public Result Update(string name, bool isMandatory, ComplianceDocumentTarget documentTarget)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Fail(ValidationMessages.RequiredMsg(nameof(Name)));
        Name = name.Trim();
        IsMandatory = isMandatory;
        DocumentTarget = documentTarget;
        return Result.Ok();
    }
}
