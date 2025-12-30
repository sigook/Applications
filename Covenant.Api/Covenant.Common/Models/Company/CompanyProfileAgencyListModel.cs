namespace Covenant.Common.Models.Company
{
    public class CompanyProfileAgencyListModel
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public string AgencyFullName { get; set; }
        public string AgencyLogo { get; set; }
        public bool Active { get; set; }
    }
}