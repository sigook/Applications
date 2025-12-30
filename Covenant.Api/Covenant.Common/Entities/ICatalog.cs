namespace Covenant.Common.Entities
{
    public interface ICatalog<T>
    {
        T Id { get; set; }
        string Value { get; set; }
    }
}
