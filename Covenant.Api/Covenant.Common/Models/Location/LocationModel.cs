using Covenant.Common.Entities;

namespace Covenant.Common.Models.Location
{
    public class LocationModel : ILocation<CityModel>
    {
        public Guid? Id { get; set; }
        public string Address { get; set; }
        public CityModel City { get; set; }
        public string PostalCode { get; set; }
        public bool IsBilling { get; set; }
    }
}
