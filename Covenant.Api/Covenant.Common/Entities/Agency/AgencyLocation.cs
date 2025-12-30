namespace Covenant.Common.Entities.Agency
{
    public class AgencyLocation
    {
        private AgencyLocation()
        {
        }

        public AgencyLocation(Location location, Guid agencyId, bool isBilling = false)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            LocationId = location.Id;
            AgencyId = agencyId;
            IsBilling = isBilling;
        }

        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        public Guid AgencyId { get; set; }
        public Agency Agency { get; set; }
        public bool IsBilling { get; set; }
    }
}