using Covenant.Common.Repositories;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Shared;

namespace Covenant.Core.BL.Services.Shared;

public class SkillService(ICatalogRepository catalogRepository) : ISkillService
{
    private const int MinimumLength = 2;
    private static readonly HashSet<string> MeaninglessValues = ["na", "n/a", "none"];

    public async Task<List<string>> Canonicalize(IEnumerable<string> skills)
    {
        var values = (skills ?? [])
            .Select(s => s.CollapseWhitespace())
            .Where(s => s.Length >= MinimumLength && !MeaninglessValues.Contains(s.ToComparisonKey()))
            .ToList();
        if (values.Count == 0) return [];

        var catalog = await catalogRepository.GetSkillCatalog();
        var canonicalByKey = catalog
            .SelectMany(s => s.Aliases.Prepend(s.Value).Select(alias => (Key: alias.ToComparisonKey(), s.Value)))
            .DistinctBy(e => e.Key)
            .ToDictionary(e => e.Key, e => e.Value);

        return values
            .Select(s => canonicalByKey.GetValueOrDefault(s.ToComparisonKey()) ?? s)
            .DistinctBy(s => s.ToComparisonKey())
            .ToList();
    }
}
