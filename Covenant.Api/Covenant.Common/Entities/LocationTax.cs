namespace Covenant.Common.Entities;

public class LocationTax
{
    public Guid LocationId { get; internal set; }
    public decimal Tax1 { get; internal set; }
    public Location Location { get; private set; }

    internal LocationTax()
    {
    }

    public LocationTax(Guid locationId, decimal tax1)
    {
        LocationId = locationId;
        Tax1 = tax1;
    }

    public void UpdateTax(decimal tax1) => Tax1 = tax1;
}
