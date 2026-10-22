using Covenant.Common.Models.Workers;
namespace Covenant.Common.Models.Accounting.PayStubs;

public class PayStubItemHistoryModel
{
    public string Description { get; set; }
    public double Quantity { get; set; }
    public decimal Total { get; set; }
}
