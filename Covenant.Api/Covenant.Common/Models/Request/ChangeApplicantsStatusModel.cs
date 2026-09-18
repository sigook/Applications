using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request;

public class ChangeApplicantsStatusModel
{
    public List<Guid> ApplicantIds { get; set; } = [];
    public RequestApplicantStatus Status { get; set; }
}
