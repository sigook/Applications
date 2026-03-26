namespace Covenant.Common.Entities.Accounting.PayStub;

public class PayStubOtherDeduction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string Description { get; set; }
    public Guid PayStubId { get; set; }
    public PayStub PayStub { get; set; }

    public static PayStubOtherDeduction CreateDefaultDeduction(decimal unitPrice, string description) => new()
    {
        Quantity = 1,
        UnitPrice = unitPrice,
        Description = description,
        Total = decimal.Multiply(unitPrice, 1m)
    };
}