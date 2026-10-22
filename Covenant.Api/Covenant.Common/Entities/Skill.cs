namespace Covenant.Common.Entities;

public class Skill : ICatalog<Guid>
{
    public Guid Id { get; private set; }
    public string Value { get; private set; }
    public List<string> Aliases { get; private set; } = [];
}
