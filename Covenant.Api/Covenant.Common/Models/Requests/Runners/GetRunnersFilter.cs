using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Runners;

public enum GetRunnersSortBy : byte
{
    Name,
    Status,
    Type,
    CreatedAt
}

public class GetRunnersFilter : Pagination
{
    public Guid? RequestId { get; set; }
    public string Name { get; set; }
    public RunnerType? Type { get; set; }
    public List<RunnerStatus> Statuses { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public GetRunnersSortBy SortBy { get; set; }
}
