namespace Covenant.Common.Entities
{
    public class Availability : ICatalog<Guid>
    {
        public Availability()
        {
        }

        public Availability(string value, Guid id = default)
        {
            Value = value;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; }
    }
}