using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.Workers;

public class GetWorkerHistoryFilter : Pagination
{
    public WorkerAssignmentStatus? Status { get; set; }
}
