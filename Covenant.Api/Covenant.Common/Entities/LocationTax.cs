namespace Covenant.Common.Entities;

public class LocationTax
{
    public Guid LocationId { get; set; }
    public decimal Tax1 { get; set; }
    public Location Location { get; set; }
}
