using Covenant.Common.Enums;


namespace Covenant.Common.Models.Company
{
    public enum GetCompanyForAgencySortBy
    {
        Name,
        Industry,
        CreatedAt,
        UpdatedAt,
        SalesRepresentative
    }

    public class GetCompanyForAgencyFilter : Pagination
    {
        public string BusinessInfo { get; set; }
        public string ContactInfo { get; set; }
        public string Industry { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAtFrom { get; set; }
        public DateTime? UpdatedAtTo { get; set; }
        public GetCompanyForAgencySortBy SortBy { get; set; }
        public IEnumerable<CompanyStatus> CompanyStatuses { get; set; }
        public string SalesRepresentative { get; set; }
    }
}