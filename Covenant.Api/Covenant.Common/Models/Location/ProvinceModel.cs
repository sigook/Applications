using Covenant.Common.Entities;

namespace Covenant.Common.Models.Location
{
    public class ProvinceModel : ICatalog<Guid>
    {
        public ProvinceModel()
        {
        }

        public ProvinceModel(Guid id, string value, string code)
        {
            Id = id;
            Value = value;
            Code = code;
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public ProvinceSettingsModel Settings { get; set; }
        public CountryModel Country { get; set; }
    }
}
