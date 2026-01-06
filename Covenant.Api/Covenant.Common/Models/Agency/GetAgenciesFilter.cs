using Covenant.Common.Enums;
using Covenant.Common.Models;

namespace Covenant.Common.Models.Agency;

public enum GetAgenciesSortBy
{
    FullName,
    Email
}

public class GetAgenciesFilter : Pagination
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public IEnumerable<AgencyType> AgencyTypes { get; set; }
    public GetAgenciesSortBy SortBy { get; set; }
}
