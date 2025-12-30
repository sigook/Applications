namespace Covenant.Common.Entities.Accounting.PayStub
{
    public interface IPayStubDeduction
    {
        Guid Id { get; }
        double Quantity { get; }
        decimal UnitPrice { get; }
        decimal Total { get; }
        string Description { get; }
    }
}