namespace Covenant.Common.Entities
{
    public class City : ICatalog<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; }
        public string Code { get; set; }
        public Guid ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}
