namespace Covenant.Common.Models.Company
{
    public class CompanyProfileIndustryDetailModel
    {
        public Guid Id { get; set; }
        public BaseModel<Guid> Industry { get; set; }
        public string OtherIndustry { get; set; }
    }
}
