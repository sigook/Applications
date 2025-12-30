using Covenant.Common.Entities;

namespace Covenant.Common.Models
{
    public class BaseModel<T> : ICatalog<T>
    {
        public BaseModel()
        {
        }

        public BaseModel(T id)
        {
            Id = id;
        }

        public BaseModel(T id, string value)
        {
            Id = id;
            Value = value;
        }

        public T Id { get; set; }

        public string Value { get; set; }

        protected bool Equals(BaseModel<T> other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BaseModel<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
