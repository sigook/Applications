using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request;

// Requests are always listed by their number (IsDescending decides the
// direction, as in the requests grid); this only sorts the applicants inside.
public enum GetAgencyApplicantSortBy : byte
{
    Name,
    CreatedAt
}

public class GetAgencyApplicantsFilter : Pagination
{
    public string Name { get; set; }
    public int? NumberId { get; set; }
    public string JobTitle { get; set; }
    public Guid? CompanyProfileId { get; set; }
    public List<RequestApplicantStatus> Statuses { get; set; } = [];
    public List<RequestStatus> RequestStatuses { get; set; } = [];
    public bool OnlyMine { get; set; }
    public string Recruiter { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? StartAtFrom { get; set; }
    public DateTime? StartAtTo { get; set; }
    public GetAgencyApplicantSortBy SortBy { get; set; }
}
