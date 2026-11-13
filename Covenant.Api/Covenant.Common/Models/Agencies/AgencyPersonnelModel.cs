namespace Covenant.Common.Models.Agencies;

public class AgencyPersonnelModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public List<Guid> AgencyIds { get; set; } = [];
    public List<string> Agencies { get; set; } = [];
}
