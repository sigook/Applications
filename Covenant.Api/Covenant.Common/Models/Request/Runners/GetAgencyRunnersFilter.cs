using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Runners;

public enum GetAgencyRunnerSortBy : byte
{
    Name,
    CreatedAt,
    Status
}

public class GetAgencyRunnersFilter : Pagination
{
    public string Name { get; set; }
    public int? NumberId { get; set; }
    public string JobTitle { get; set; }
    public Guid? CompanyProfileId { get; set; }
    public RunnerType? Type { get; set; }
    public List<RunnerStatus> Statuses { get; set; } = [];
    public List<RequestStatus> RequestStatuses { get; set; } = [];
    public bool OnlyMine { get; set; }
    public string Recruiter { get; set; }
    public DateTime? StartAtFrom { get; set; }
    public DateTime? StartAtTo { get; set; }
    public GetAgencyRunnerSortBy SortBy { get; set; }
}
