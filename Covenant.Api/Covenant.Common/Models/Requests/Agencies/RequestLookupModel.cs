using Covenant.Common.Models.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Locations;

namespace Covenant.Common.Models.Requests.Agencies;

public class RequestLookupModel
{
    public AgencyRequestDetailModel Request { get; set; }
    public IEnumerable<CompanyProfileJobPositionRateModel> JobPositions { get; set; }
    public IEnumerable<LocationDetailModel> Locations { get; set; }
    public IEnumerable<AgencyPersonnelModel> Personnel { get; set; }
    public IEnumerable<CompanyUserModel> CompanyUsers { get; set; }
}
