namespace Covenant.Common.Entities.Company
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

        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        public Guid CompanyProfileId { get; set; }
        public CompanyProfile CompanyProfile { get; internal set; }
        public bool IsBilling { get; set; }
    }
}