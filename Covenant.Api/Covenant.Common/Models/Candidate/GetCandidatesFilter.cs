namespace Covenant.Common.Models.Candidate
{
    public enum GetCandidatesSortBy
    {
        Name,
        Address,
        Skills,
        CreateAt,
        Recruiter,
        Status,
        Source
    }

    public class GetCandidatesFilter : Pagination
    {
        public GetCandidatesSortBy SortBy { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Skills { get; set; }
        public int? NumberId { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public string Recruiter { get; set; }
        public string[] Statuses { get; set; }
        public string Source { get; set; }
        public bool ResumeOnly { get; set; }
    }
}
