using System.Collections.Concurrent;
using Sigook.Mcp.Models;
using Sigook.Mcp.Tools;

namespace Sigook.Mcp.Api;

public sealed record CityAlternative(Guid Id, string Value, double Score);

public sealed record CityResolution(CityModel? City, ProvinceModel? Province, string? Error, List<ProvinceModel>? Provinces, List<CityAlternative>? Alternatives);

public sealed class CatalogCache(ISigookApi api)
{
    private readonly ConcurrentDictionary<string, Task<List<CatalogItem>>> sources = new();
    private readonly ConcurrentDictionary<string, Task<List<ProvinceModel>>> provinces = new();
    private readonly ConcurrentDictionary<Guid, Task<List<CityModel>>> cities = new();
    private readonly ConcurrentDictionary<int, Task<AgencyRequestListModel?>> requests = new();

    public Task<List<CatalogItem>> Sources() => Cached(sources, string.Empty, () => api.GetSources(CancellationToken.None).OrThrow());

    public Task<List<ProvinceModel>> Provinces(string countryCode) => Cached(provinces, countryCode, () => api.GetProvinces(countryCode, CancellationToken.None).OrThrow());

    public Task<List<CityModel>> Cities(Guid provinceId) => Cached(cities, provinceId, () => api.GetCities(provinceId, CancellationToken.None).OrThrow());

    public Task<AgencyRequestListModel?> Request(int numberId) => Cached(requests, numberId, async () =>
    {
        var page = await api.GetRequests(numberId, 5, CancellationToken.None).OrThrow();
        return page.Items.FirstOrDefault(r => r.NumberId == numberId);
    });

    public async Task<Guid?> SourceId(string? name)
    {
        var all = await Sources();
        var match = string.IsNullOrWhiteSpace(name) ? null : all.FirstOrDefault(s => LookupTools.Same(s.Value, name));
        return (match ?? all.FirstOrDefault(s => LookupTools.Same(s.Value, "Other")))?.Id;
    }

    public async Task<CityResolution> ResolveCity(string city, string provinceOrState, string countryCode)
    {
        var code = countryCode.Trim().ToUpperInvariant() switch { "US" => "USA", var c => c };
        var all = await Provinces(code);
        var province = all.FirstOrDefault(p => LookupTools.Same(p.Code, provinceOrState) || LookupTools.Same(p.Value, provinceOrState));
        if (province is null)
            return new(null, null, $"Province/state '{provinceOrState}' not found in {code}", all, null);
        var list = await Cities(province.Id);
        var exact = list.FirstOrDefault(c => LookupTools.Same(c.Value, city));
        if (exact is not null)
            return new(new CityModel(exact.Id, exact.Value, exact.Code, province), province, null, null, null);
        var needle = LookupTools.Fold(city);
        var alternatives = list
            .Select(c => new CityAlternative(c.Id, c.Value, LookupTools.Similarity(LookupTools.Fold(c.Value), needle)))
            .OrderByDescending(c => c.Score)
            .Take(5)
            .ToList();
        return new(null, province, $"City '{city}' not found in {province.Value}", null, alternatives);
    }

    private static Task<T> Cached<TKey, T>(ConcurrentDictionary<TKey, Task<T>> cache, TKey key, Func<Task<T>> factory) where TKey : notnull
    {
        if (cache.TryGetValue(key, out var existing) && !existing.IsFaulted && !existing.IsCanceled) return existing;
        var created = factory();
        cache[key] = created;
        return created;
    }
}
