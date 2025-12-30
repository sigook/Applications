namespace Covenant.Common.Entities
{
    public class Day
    {
        public Day()
        {
        }

        public Day(string value, Guid id = default)
        {
            Id = id == default ? Guid.NewGuid() : id;
            Value = value;
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}