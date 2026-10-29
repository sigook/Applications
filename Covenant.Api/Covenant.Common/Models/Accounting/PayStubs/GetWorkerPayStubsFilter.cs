namespace Covenant.Common.Models.Accounting.PayStubs;

public class GetWorkerPayStubsFilter : Pagination
{
    public int? Year { get; set; }
}
