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

        public Guid LocationId { get; private set; }
        public Location Location { get; private set; }
        public Guid AgencyId { get; private set; }
        public Agency Agency { get; private set; }
        public bool IsBilling { get; internal set; }
    }
}