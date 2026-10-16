using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Agency;

public enum WorkerRequestHistorySortBy : byte
{
    NumberId,
    Client,
    JobTitle,
    StartWorking,
    FinishWorking,
    CreatedAt
}

public class GetWorkerRequestHistoryFilter : Pagination
{
    public int? NumberId { get; set; }
    public string CompanyFullName { get; set; }
    public string JobTitle { get; set; }
    public string Location { get; set; }
    public IEnumerable<RequestStatus> Statuses { get; set; }
    public IEnumerable<WorkerRequestStatus> WorkerRequestStatuses { get; set; }
    public DateTime? StartWorkingFrom { get; set; }
    public DateTime? StartWorkingTo { get; set; }
    public WorkerRequestHistorySortBy? SortBy { get; set; }
}
