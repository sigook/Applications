using Covenant.Infrastructure.Contexts;

namespace Covenant.Integration.Tests.Configuration;

public class SeededWebApplicationFactory<TStartup, TData> : CustomWebApplicationFactory<TStartup>
    where TStartup : class
    where TData : ITestData, new()
{
    private readonly Lazy<TData> data;

    public SeededWebApplicationFactory() =>
        data = new Lazy<TData>(Seed, LazyThreadSafetyMode.ExecutionAndPublication);

    public TData Data => data.Value;

    private TData Seed()
    {
        var seed = new TData();
        seed.Seed(Services.GetRequiredService<CovenantContext>());
        return seed;
    }
}
