using Covenant.Common.Repositories;
using Xunit;

namespace Covenant.Tests.Architecture;

public class EntityEncapsulationTest
{
    private const string EntitiesNamespace = "Covenant.Common.Entities";

    [Fact]
    public void EntitiesHaveNoPublicSetters()
    {
        var violations = typeof(IBaseRepository).Assembly.GetTypes()
            .Where(t => t.IsClass && t.Namespace is not null && t.Namespace.StartsWith(EntitiesNamespace))
            .SelectMany(t => t.GetProperties()
                .Where(p => p.DeclaringType == t && p.SetMethod is { IsPublic: true })
                .Select(p => $"{t.FullName}.{p.Name}"))
            .ToList();

        Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
    }
}
