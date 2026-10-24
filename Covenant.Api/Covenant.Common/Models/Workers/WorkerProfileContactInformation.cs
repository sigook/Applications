using Covenant.Common.Models.Locations;

namespace Covenant.Common.Models.Workers;

public class WorkerProfileContactInformation : IWorkerContactInformation<LocationModel, CityModel>
{
    public string MobileNumber { get; set; }
    public string Phone { get; set; }
    public int? PhoneExt { get; set; }
    public LocationModel Location { get; set; }
}
