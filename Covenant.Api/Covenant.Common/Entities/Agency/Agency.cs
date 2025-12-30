using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Agency;

public class Agency
{
    public Agency()
    {
    }

    public Agency(string fullName, string phonePrincipal)
    {
        FullName = fullName;
        PhonePrincipal = phonePrincipal;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public int NumberId { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string HstNumber { get; set; }
    public string BusinessNumber { get; set; }
    public Guid? LogoId { get; set; }
    public CovenantFile Logo { get; set; }
    public string WebPage { get; set; }
    public string PhonePrincipal { get; set; }
    public int? PhonePrincipalExt { get; set; }
    public string RecruitmentEmail { get; set; }
    public AgencyType AgencyType { get; set; }
    public Guid? AgencyParentId { get; set; }
    public Agency AgencyParent { get; set; }
    public ICollection<AgencyWsibGroup> WsibGroup { get; set; } = new List<AgencyWsibGroup>();
    public ICollection<AgencyLocation> Locations { get; set; } = new List<AgencyLocation>();
    public ICollection<AgencyContactInformation> ContactInformation { get; set; } = new List<AgencyContactInformation>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Location BillingAddress => Locations.FirstOrDefault(l => l.IsBilling)?.Location ?? Locations.FirstOrDefault()?.Location;
    public string FormattedPhone => $"{PhonePrincipal} {(PhonePrincipalExt.HasValue ? $"Ext:{PhonePrincipalExt.Value}" : string.Empty)}";

    public void AddLocation(Location location, bool isBilling = false)
    {
        Locations.Add(new AgencyLocation(location, Id, isBilling));
    }

    public Result UpdateLocation(Guid id, Location location, bool isBilling = false)
    {
        AgencyLocation toUpdate = Locations.SingleOrDefault(s => s.LocationId == id);
        if (toUpdate is null) return Result.Fail("Location not found");
        toUpdate.IsBilling = isBilling;
        return toUpdate.Location.Update(location);
    }

    public Result DeleteLocation(Guid id)
    {
        AgencyLocation toDelete = Locations.SingleOrDefault(s => s.LocationId == id);
        if (toDelete is null) return Result.Fail("Location not found");
        Locations.Remove(toDelete);
        return Result.Ok();
    }
}