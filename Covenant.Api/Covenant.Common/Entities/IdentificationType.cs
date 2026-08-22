using Covenant.Common.Enums;

namespace Covenant.Common.Entities
{
    public class IdentificationType : ICatalog<Guid>
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public IdentificationTypeCode Code { get; set; }
        public IdentificationType()
        {
        }

        public IdentificationType(string value, Guid id = default)
        {
            Value = value;
            Id = id == default ? Guid.NewGuid() : id;
        }
    }
}
