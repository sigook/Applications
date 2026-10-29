namespace Covenant.Common.Models.Companies.Agencies;

public enum GetJobPositionsSortBy
{
    Role,
    AgencyRate,
    WorkerRate,
    CreatedAt
}

public class GetJobPositionsFilter : Pagination
{
    public string Role { get; set; }
    public GetJobPositionsSortBy SortBy { get; set; }
}
