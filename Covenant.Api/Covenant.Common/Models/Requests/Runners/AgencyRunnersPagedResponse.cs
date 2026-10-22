namespace Covenant.Common.Models.Requests.Runners;

public class AgencyRunnersPagedResponse : PaginatedList<AgencyRequestRunnersModel>
{
    public int TotalRunners { get; set; }
}
