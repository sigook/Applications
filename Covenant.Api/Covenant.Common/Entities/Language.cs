using System;

namespace Covenant.Common.Entities
{
    public class Language
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; }
        public Language()
        {
        }
        public Language(string value, Guid id = default)
        {
            Value = value;
            Id = id == default ? Guid.NewGuid() : id;
        }
    }
}