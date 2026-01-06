using Covenant.Common.Models.Location;

namespace Covenant.Common.Models.Company
{
    public class CompanyProfileLocationDetailModel : LocationModel
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Entrance { get; set; }
        public string MainIntersection { get; set; }
        public ProvinceModel Province { get; set; }
    }
}