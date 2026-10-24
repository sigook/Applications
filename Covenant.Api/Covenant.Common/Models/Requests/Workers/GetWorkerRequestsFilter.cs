namespace Covenant.Common.Models.Requests.Workers;

public enum GetWorkerRequestsSortBy : byte
{
    NumberId,
    Rate
}

public class GetWorkerRequestsFilter : Pagination
{
    public override bool IsDescending { get; set; } = true;
    public GetWorkerRequestsSortBy SortBy { get; set; }
    public int? NumberId { get; set; }
    public string JobTitle { get; set; }
}
