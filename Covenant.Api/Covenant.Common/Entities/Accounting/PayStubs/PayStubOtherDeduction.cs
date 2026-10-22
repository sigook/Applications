namespace Covenant.Common.Entities.Accounting.PayStub;

public class PayStubOtherDeduction
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public double Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }
    public string Description { get; private set; }
    public Guid PayStubId { get; private set; }
    public PayStub PayStub { get; private set; }

    public static PayStubOtherDeduction CreateDefaultDeduction(decimal unitPrice, string description) => new()
    {
        Quantity = 1,
        UnitPrice = unitPrice,
        Description = description,
        Total = decimal.Multiply(unitPrice, 1m)
    };
}