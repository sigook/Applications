using Covenant.Common.Entities;

namespace Covenant.Common.Models.Location
{
    public class CountryModel : ICatalog<Guid>
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
    }
}
