namespace Covenant.Common.Entities
{
    public class Gender : ICatalog<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; }
        public Gender()
        {
        }
        public Gender(string value, Guid id = default)
        {
            Value = value;
            Id = id == default ? Guid.NewGuid() : id;
        }
    }
}