namespace Covenant.Common.Models.Accounting.PayStub;

public enum WorkerPayStubHistorySortBy : byte
{
    PayStubNumber,
    WeekEnding,
    TotalEarnings,
    TotalPaid
}

public class GetWorkerPayStubHistoryFilter : Pagination
{
    public string PayStubNumber { get; set; }
    public DateTime? WeekEndingFrom { get; set; }
    public DateTime? WeekEndingTo { get; set; }
    public WorkerPayStubHistorySortBy? SortBy { get; set; }
}
