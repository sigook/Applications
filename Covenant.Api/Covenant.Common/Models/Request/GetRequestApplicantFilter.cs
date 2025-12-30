namespace Covenant.Common.Models.Request
{
    public enum GetRequestApplicantSortBy : byte
    {
        Name,
        CreatedAt
    }

    public class GetRequestApplicantFilter : Pagination
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public GetRequestApplicantSortBy SortBy { get; set; }
    }
}
