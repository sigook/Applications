using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.PayStubs;

public class CreatePayStubItemModel
{
    public PayStubItemType Type { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Description { get; set; }
}
