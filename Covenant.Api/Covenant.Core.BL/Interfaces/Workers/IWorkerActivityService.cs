using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Models.Workers;

namespace Covenant.Core.BL.Interfaces.Workers;

public interface IWorkerActivityService
{
    Task<WorkerWeekTimeSheetsModel> GetWeekTimeSheets(Guid workerId, DateTime weekStart);
    Task<WorkerActivitySummaryModel> GetSummary(Guid workerId, DateTime weekStart);
}
