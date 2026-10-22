using Covenant.Common.Models.Companies.Agencies;

namespace Covenant.Common.Models.Companies;

public class CompanyProfileWithDetailsModel
{
    public CompanyProfileListModel Company { get; set; }
    public IEnumerable<CompanyUserModel> Users { get; set; }
    public IEnumerable<CompanyProfileContactPersonModel> Contacts { get; set; }
    public IEnumerable<CompanyProfileJobPositionRateModel> JobPositions { get; set; }
    public IEnumerable<CompanyProfileLocationListModel> Locations { get; set; }
}
