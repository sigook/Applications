namespace Covenant.Common.Models.Request.Runners;

public class AgencyRunnersPagedResponse : PaginatedList<AgencyRequestRunnersModel>
{
    public int TotalRunners { get; set; }
}
