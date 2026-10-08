using Covenant.Common.Enums;
using Covenant.Common.Models.Company;

namespace Covenant.Common.Models.Sales;

public class CreateDealModel
{
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public DealType Type { get; set; }
    public DealStatus Status { get; set; }
    public Guid? DocumentId { get; set; }
    public string FileName { get; set; }
}
