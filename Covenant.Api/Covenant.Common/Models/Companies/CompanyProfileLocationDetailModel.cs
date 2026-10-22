using Covenant.Common.Models.Locations;

namespace Covenant.Common.Models.Companies
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