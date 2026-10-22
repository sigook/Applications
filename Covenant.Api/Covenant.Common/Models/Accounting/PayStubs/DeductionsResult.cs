namespace Covenant.Common.Models.Accounting.PayStub;

public class DeductionsResult
{
    public decimal Cpp { get; init; }
    public decimal Ei { get; init; }
    public decimal FederalTax { get; init; }
    public decimal ProvincialTax { get; init; }
}
