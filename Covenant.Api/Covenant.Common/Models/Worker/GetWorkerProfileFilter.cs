namespace Covenant.Common.Models.Worker
{
    public enum GetWorkersProfileSortBy
    {
        Name,
        NumberId,
        RequestId,
        CreatedAt,
        Skills
    }

    public enum WorkersProfileFeature
    {
        Working,
        NotWorking,
        Dnu,
        ApprovedToWork,
        Subcontractor
    }

    public class GetWorkerProfileFilter : Pagination
    {
        public GetWorkersProfileSortBy SortBy { get; set; }
        public bool ApprovedToWork { get; set; }
        public bool? IsSubcontractor { get; set; }
        public int? NumberId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string RequestId { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public string Skills { get; set; }
        public IEnumerable<WorkersProfileFeature> Features { get; set; }
        public Guid? CompanyProfileId { get; set; }
    }
}