namespace Covenant.Common.Models.Agencies;

public class AgencyUserContextModel
{
    public Guid AgencyId { get; init; }
    public Guid AgencyPersonnelId { get; init; }
    public List<Guid> AgencyIds { get; init; } = [];
}
