using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileContactInformation : IWorkerContactInformation<LocationModel, CityModel>
    {
        public string MobileNumber { get; set; }
        public string Phone { get; set; }
        public int? PhoneExt { get; set; }
        public LocationModel Location { get; set; }
    }
}