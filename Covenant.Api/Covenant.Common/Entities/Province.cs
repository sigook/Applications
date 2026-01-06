namespace Covenant.Common.Entities
{
    public class Province
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; }
        public string Code { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public ProvinceTax ProvinceTax { get; set; }
        public ProvinceSetting ProvinceSetting { get; set; }
    }
}
