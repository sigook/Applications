using Covenant.Common.Entities.Company;

namespace Covenant.Common.Models.Company;

public class UpdateDealModel
{
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public Deal.DealType Type { get; set; }
    public Deal.DealStatus Status { get; set; }
    public Guid? DocumentId { get; set; }
}
