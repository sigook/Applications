namespace Covenant.Common.Entities
{
    public interface ICatalog<T>
    {
        T Id { get; }
        string Value { get; }
    }
}
