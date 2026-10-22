using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Request;

public class RequestApplicantComplianceItem
{
    private RequestApplicantComplianceItem()
    {
    }

    public Guid Id { get; internal set; } = Guid.NewGuid();
    public Guid RequestApplicantId { get; private set; }
    public RequestApplicant RequestApplicant { get; private set; }
    public Guid RequestComplianceItemId { get; private set; }
    public RequestComplianceItem RequestComplianceItem { get; private set; }
    public DateTime CompletedAt { get; private set; } = DateTime.Now;
    public string CompletedBy { get; private set; }

    public static Result<RequestApplicantComplianceItem> Create(Guid requestApplicantId, Guid requestComplianceItemId, string completedBy) =>
        Result.Ok(new RequestApplicantComplianceItem
        {
            RequestApplicantId = requestApplicantId,
            RequestComplianceItemId = requestComplianceItemId,
            CompletedBy = completedBy
        });
}
