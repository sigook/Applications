namespace Covenant.Common.Entities
{
    public class Lift : ICatalog<Guid>
    {
        public Lift()
        {
        }

        public Lift(string value, Guid id = default)
        {
            Value = value;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}