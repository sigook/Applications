namespace Covenant.Common.Entities.Companies
{
    public class CompanyProfileLocation
    {
        public CompanyProfileLocation()
        {
        }

        public CompanyProfileLocation(Guid companyProfileId, Location location)
        {
            CompanyProfileId = companyProfileId;
            Location = location ?? throw new ArgumentNullException(nameof(location));
            LocationId = location.Id;
        }

        public Guid LocationId { get; internal set; }
        public Location Location { get; internal set; }
        public Guid CompanyProfileId { get; internal set; }
        public CompanyProfile CompanyProfile { get; internal set; }
        public bool IsBilling { get; internal set; }

        public void UpdateIsBilling(bool isBilling) => IsBilling = isBilling;
    }
}
