namespace Covenant.Common.Entities;

public class ProvinceTax
{
    public Guid ProvinceId { get; set; }
    public decimal Tax1 { get; set; }
    public Province Province { get; set; }
}
