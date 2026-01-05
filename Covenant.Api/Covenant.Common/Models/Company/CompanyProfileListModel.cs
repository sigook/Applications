using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company
{
    public class CompanyProfileListModel
    {
        public Guid Id { get; set; }
        public string Logo { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public int NumberId { get; set; }
        public IEnumerable<string> Locations { get; set; }
        public bool Active { get; set; }
        public Guid CompanyId { get; set; }
        public Guid AgencyId { get; set; }
        public string Industry { get; set; }
        public CompanyStatus CompanyStatus { get; set; }
        public string ContactName { get; set; }
        public string ContactRole { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int NotesCount { get; set; }
        public string SalesRepresentative { get; set; }
    }
}
