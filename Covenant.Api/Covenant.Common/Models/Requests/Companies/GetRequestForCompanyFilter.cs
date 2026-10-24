using Covenant.Common.Models.Requests.Agencies;

namespace Covenant.Common.Models.Requests.Companies;

public class GetRequestForCompanyFilter : Pagination
{
    public Guid? CompanyUserId { get; set; }
    public GetRequestSortBy SortBy { get; set; }
    public int? NumberId { get; set; }
    public string JobTitle { get; set; }
    public string Location { get; set; }
}
