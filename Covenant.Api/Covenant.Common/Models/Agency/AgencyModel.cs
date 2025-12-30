using Covenant.Common.Enums;
using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Agency;

public class AgencyModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string HstNumber { get; set; }
    public string BusinessNumber { get; set; }
    public string WebPage { get; set; }
    public string PhonePrincipal { get; set; }
    public int? PhonePrincipalExt { get; set; }
    public CovenantFileModel Logo { get; set; }
    public string Email { get; set; }
    public AgencyType AgencyType { get; set; }
    public IEnumerable<BaseModel<Guid>> WsibGroup { get; set; } = Array.Empty<BaseModel<Guid>>();
    public IEnumerable<LocationDetailModel> Locations { get; set; } = Array.Empty<LocationDetailModel>();
    public IEnumerable<AgencyContactInformationModel> ContactInformation { get; set; } = Array.Empty<AgencyContactInformationModel>();
}