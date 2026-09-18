namespace Covenant.Common.Models.Request;

// The page is a page of requests: a request always brings every applicant that
// matches the filter, so no request is ever split across pages.
public class AgencyApplicantsPagedResponse : PaginatedList<AgencyRequestApplicantsModel>
{
    public int TotalApplicants { get; set; }
}
