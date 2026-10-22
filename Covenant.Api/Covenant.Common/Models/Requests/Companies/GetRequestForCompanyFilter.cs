using Covenant.Common.Models.Request.Agency;

namespace Covenant.Common.Models.Request.Company;

public class GetRequestForCompanyFilter : Pagination
{
    public Guid? CompanyUserId { get; set; }
    public GetRequestSortBy SortBy { get; set; }
    public int? NumberId { get; set; }
    public string JobTitle { get; set; }
    public string Location { get; set; }
}
