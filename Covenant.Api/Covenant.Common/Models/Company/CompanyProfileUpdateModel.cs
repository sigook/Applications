namespace Covenant.Common.Models.Company
{
    public class CompanyProfileUpdateModel
    {
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string Phone { get; set; }
        public int? PhoneExt { get; set; }
        public string Fax { get; set; }
        public int? FaxExt { get; set; }
        public string Website { get; set; }
        public CompanyProfileIndustryDetailModel Industry { get; set; }
    }
}