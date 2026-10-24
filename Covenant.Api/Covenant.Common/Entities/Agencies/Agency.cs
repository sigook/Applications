using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Agencies;

public class Agency
{
    public const string DefaultImageLogo = "agency.png";

    public Agency()
    {
    }

    public Agency(string fullName, string phonePrincipal)
    {
        FullName = fullName;
        PhonePrincipal = phonePrincipal;
    }

    public Guid Id { get; internal set; } = Guid.NewGuid();
    public int NumberId { get; private set; }
    public User User { get; internal set; }
    public Guid UserId { get; private set; }
    public string FullName { get; private set; }
    public string HstNumber { get; private set; }
    public string BusinessNumber { get; private set; }
    public Guid? LogoId { get; private set; }
    public CovenantFile Logo { get; private set; }
    public string WebPage { get; private set; }
    public string PhonePrincipal { get; private set; }
    public int? PhonePrincipalExt { get; private set; }
    public string RecruitmentEmail { get; internal set; }
    public AgencyType AgencyType { get; internal set; }
    public Guid? AgencyParentId { get; internal set; }
    public Agency AgencyParent { get; private set; }
    public ICollection<AgencyWsibGroup> WsibGroup { get; private set; } = new List<AgencyWsibGroup>();
    public ICollection<AgencyLocation> Locations { get; private set; } = new List<AgencyLocation>();
    public ICollection<AgencyContactInformation> ContactInformation { get; private set; } = new List<AgencyContactInformation>();
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public Location BillingAddress => Locations.FirstOrDefault(l => l.IsBilling)?.Location ?? Locations.FirstOrDefault()?.Location;
    public string FormattedPhone => $"{PhonePrincipal} {(PhonePrincipalExt.HasValue ? $"Ext:{PhonePrincipalExt.Value}" : string.Empty)}";

    public void UpdateProfile(string fullName, string businessNumber, string hstNumber, string phonePrincipal, int? phonePrincipalExt, string webPage)
    {
        FullName = fullName;
        BusinessNumber = businessNumber;
        HstNumber = hstNumber;
        PhonePrincipal = phonePrincipal;
        PhonePrincipalExt = phonePrincipalExt;
        WebPage = webPage;
    }

    public void ChangeLogo(CovenantFile logo)
    {
        Logo = logo;
        LogoId = logo?.Id;
    }

    public void UpdateLogo(Guid? logoId)
    {
        if (logoId.HasValue)
        {
            LogoId = logoId;
            return;
        }
        Logo ??= new CovenantFile(DefaultImageLogo);
        LogoId = Logo.Id;
    }

    public void ReplaceWsibGroups(IEnumerable<Guid> wsibGroupIds)
    {
        WsibGroup.Clear();
        foreach (var wsibGroupId in wsibGroupIds)
        {
            WsibGroup.Add(new AgencyWsibGroup(Id, wsibGroupId));
        }
    }

    public void ReplaceContactInformation(IEnumerable<AgencyContactInformation> contactInformation)
    {
        ContactInformation.Clear();
        foreach (var item in contactInformation)
        {
            ContactInformation.Add(item);
        }
    }

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

    public void AssignOwner(AgencyType agencyType, Guid userId, Guid? agencyParentId)
    {
        AgencyType = agencyType;
        UserId = userId;
        AgencyParentId = agencyParentId;
    }
}
