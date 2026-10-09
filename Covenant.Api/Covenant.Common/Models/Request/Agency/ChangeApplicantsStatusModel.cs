using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Agency;

public class ChangeApplicantsStatusModel
{
    public List<Guid> ApplicantIds { get; set; } = [];
    public RequestApplicantStatus Status { get; set; }
}
