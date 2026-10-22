using Covenant.Common.Models.Worker;
namespace Covenant.Common.Models.Accounting.PayStub;

public class PayStubItemHistoryModel
{
    public string Description { get; set; }
    public double Quantity { get; set; }
    public decimal Total { get; set; }
}
