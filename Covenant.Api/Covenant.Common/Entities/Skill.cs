namespace Covenant.Common.Entities;

public class Skill : ICatalog<Guid>
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public List<string> Aliases { get; set; } = [];
}
