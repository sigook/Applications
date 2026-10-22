using Covenant.Common.Enums;

namespace Covenant.Common.Entities
{
    public class IdentificationType : ICatalog<Guid>
    {
        public Guid Id { get; private set; }
        public string Value { get; internal set; }
        public IdentificationTypeCode Code { get; internal set; }
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
