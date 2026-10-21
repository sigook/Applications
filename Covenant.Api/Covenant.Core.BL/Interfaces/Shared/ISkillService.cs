namespace Covenant.Core.BL.Interfaces.Shared;

public interface ISkillService
{
    Task<List<string>> Canonicalize(IEnumerable<string> skills);
}
