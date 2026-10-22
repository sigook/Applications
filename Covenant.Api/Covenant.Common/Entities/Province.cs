namespace Covenant.Common.Entities
{
    public class Province
    {
        public Guid Id { get; internal set; } = Guid.NewGuid();
        public string Value { get; internal set; }
        public string Code { get; internal set; }
        public Guid CountryId { get; private set; }
        public Country Country { get; internal set; }
        public ProvinceSetting ProvinceSetting { get; private set; }
    }
}
