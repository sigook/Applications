using Covenant.Common.Entities;

namespace Covenant.Common.Models.Location
{
    public class CityModel : ICatalog<Guid>, IEquatable<CityModel>
    {
        public CityModel()
        {
        }

        public CityModel(Guid id, string value)
        {
            Id = id;
            Value = value;
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public ProvinceModel Province { get; set; }

        public bool Equals(CityModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Id.Equals(other.Id) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((CityModel)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Value);

        public static bool operator ==(CityModel left, CityModel right) => Equals(left, right);

        public static bool operator !=(CityModel left, CityModel right) => !Equals(left, right);
    }
}
