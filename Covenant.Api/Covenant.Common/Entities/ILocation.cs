namespace Covenant.Common.Entities
{
    public interface ILocation<out TCity> where TCity : ICatalog<Guid>
    {
        string Address { get; }
        TCity City { get; }
        string PostalCode { get; }
    }
}
