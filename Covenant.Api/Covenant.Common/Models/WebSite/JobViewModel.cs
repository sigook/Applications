using Covenant.Common.Enums;

namespace Covenant.Common.Models.WebSite
{
    public class JobViewModel
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string NumberId { get; set; }
        public string Title { get; set; }
        public string Salary { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Responsibilities { get; set; }
        public string Shift { get; set; }
        public string CreatedBy { get; set; }
        public Guid? AgencyId { get; set; }
        public CompanyType? CompanyType { get; set; }
    }
}
