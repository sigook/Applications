namespace Covenant.Common.Models.Requests.TimeSheets.Agencies;

public enum WorkerTimeSheetHistorySortBy : byte
{
    Company,
    Request,
    Date
}

public class GetWorkerTimeSheetHistoryFilter : Pagination
{
    public string CompanyName { get; set; }
    public int? NumberId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public WorkerTimeSheetHistorySortBy? SortBy { get; set; }
}
