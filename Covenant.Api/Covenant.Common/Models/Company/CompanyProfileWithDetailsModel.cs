namespace Covenant.Common.Models.Company;

public class CompanyProfileWithDetailsModel
{
    public CompanyProfileListModel Company { get; set; }
    public IEnumerable<CompanyUserModel> Users { get; set; }
    public IEnumerable<CompanyProfileContactPersonModel> Contacts { get; set; }
    public IEnumerable<CompanyProfileJobPositionRateModel> JobPositions { get; set; }
}
