using Covenant.Common.Models.Worker;
namespace Covenant.Common.Models.Accounting.PayStub;

public class PayStubHistoryAccumulated
{
    public decimal TotalEarnings { get; set; }
    public decimal Vacations { get; set; }
    public decimal TotalPaid { get; set; }
    public double Quantity { get; set; }
    public decimal Total { get; set; }
}
