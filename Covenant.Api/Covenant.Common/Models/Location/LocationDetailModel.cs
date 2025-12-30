using Covenant.Common.Entities;

namespace Covenant.Common.Models.Location
{
    public class LocationDetailModel : ILocation<CityModel>
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public CityModel City { get; set; }
        public string PostalCode { get; set; }
        public string Entrance { get; set; }
        public string MainIntersection { get; set; }
        public bool IsBilling { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string FormattedAddress => $"{Address} {City?.Value} {City?.Province?.Code} {PostalCode}";
        public bool IsUSA => City?.Province?.Country?.Code == "USA";
    }
}
