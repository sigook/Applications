namespace Covenant.Common.Entities
{
    public class City : ICatalog<Guid>
    {
        public City()
        {
        }

        public City(string value, Guid provinceId)
        {
            Value = value;
            ProvinceId = provinceId;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Value { get; internal set; }
        public string Code { get; private set; }
        public Guid ProvinceId { get; private set; }
        public Province Province { get; internal set; }
    }
}
