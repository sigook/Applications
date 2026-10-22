namespace Covenant.Common.Models.Accounting.PayStub;

public class PayStubYtdModel
{
    public decimal Gross { get; set; }
    public decimal Vacations { get; set; }
    public decimal Earnings { get; set; }
    public decimal Cpp { get; set; }
    public decimal Ei { get; set; }
    public decimal FederalTax { get; set; }
    public decimal ProvincialTax { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalPaid { get; set; }
}
