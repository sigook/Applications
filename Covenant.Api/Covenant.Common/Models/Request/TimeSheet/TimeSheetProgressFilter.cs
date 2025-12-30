using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.TimeSheet
{
    public enum TimeSheetProgressSortBy
    {
        NumberId,
        WorkerName,
        TotalHoursApproved,
        TotalHours,
        Status
    }

    public class TimeSheetProgressFilter : Pagination
    {
        public TimeSheetProgressSortBy SortBy { get; set; }
        public int? NumberId { get; set; }
        public string WorkerName { get; set; }
        public IEnumerable<WorkerRequestStatus> Statuses { get; set; }
    }
}