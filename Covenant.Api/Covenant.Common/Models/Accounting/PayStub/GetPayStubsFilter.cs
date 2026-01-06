namespace Covenant.Common.Models.Accounting.PayStub;

public enum GetPayStubsFilterSortBy : byte
{
    PayStubNumber,
    CreatedAt,
    WorkerFullName
}

public class GetPayStubsFilter : Pagination
{
    public string PayStubNumber { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public string WorkerFullName { get; set; }

    public GetPayStubsFilterSortBy? SortBy { get; set; }
}
