using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Request;

public class RequestLookupModel
{
    public AgencyRequestDetailModel Request { get; set; }
    public IEnumerable<CompanyProfileJobPositionRateModel> JobPositions { get; set; }
    public IEnumerable<LocationDetailModel> Locations { get; set; }
    public IEnumerable<AgencyPersonnelModel> Personnel { get; set; }
    public IEnumerable<CompanyUserModel> CompanyUsers { get; set; }
}
