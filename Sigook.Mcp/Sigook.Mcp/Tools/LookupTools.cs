using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;
using Sigook.Mcp.Api;
using Sigook.Mcp.Models;

namespace Sigook.Mcp.Tools;

[McpServerToolType]
public static partial class LookupTools
{
    [McpServerTool(Name = "find_request"), Description("Finds a Sigook request (job order) of the current agency by its number, e.g. 2549. Returns id, job title, company, location and status. The id is what apply_candidate_to_request needs.")]
    public static async Task<object> FindRequest(ISigookApi api, [Description("Request number as shown in Sigook (NumberId).")] int numberId, CancellationToken cancellationToken)
    {
        var page = await api.GetRequests(numberId, 5, cancellationToken).OrThrow();
        var match = page.Items.FirstOrDefault(r => r.NumberId == numberId);
        return match is null ? new { found = false, numberId } : new { found = true, numberId, request = match };
    }

    [McpServerTool(Name = "resolve_city"), Description("Resolves a city name plus province/state into a Sigook city. Country is CA or USA. The state can be a code (PA) or a name (Pennsylvania). Returns the best match and, when ambiguous, the closest alternatives.")]
    public static async Task<object> ResolveCity(
        ISigookApi api,
        [Description("City name, e.g. Philadelphia.")] string city,
        [Description("Province or state code or name, e.g. PA, ON, Ontario.")] string provinceOrState,
        [Description("CA or USA. Default USA.")] string countryCode = "USA",
        CancellationToken cancellationToken = default)
    {
        var code = countryCode.Trim().ToUpperInvariant() switch { "US" => "USA", var c => c };
        var provinces = await api.GetProvinces(code, cancellationToken).OrThrow();
        var province = provinces.FirstOrDefault(p => Same(p.Code, provinceOrState) || Same(p.Value, provinceOrState));
        if (province is null)
            return new { found = false, reason = $"Province/state '{provinceOrState}' not found in {code}", provinces = provinces.Select(p => new { p.Id, p.Value, p.Code }) };
        var cities = await api.GetCities(province.Id, cancellationToken).OrThrow();
        var exact = cities.FirstOrDefault(c => Same(c.Value, city));
        if (exact is not null)
            return new { found = true, city = new { exact.Id, exact.Value, province = new { province.Id, province.Value, province.Code } } };
        var needle = Fold(city);
        var alternatives = cities
            .Select(c => new { c.Id, c.Value, score = Similarity(Fold(c.Value), needle) })
            .OrderByDescending(c => c.score)
            .Take(5)
            .ToList();
        return new { found = false, reason = $"City '{city}' not found in {province.Value}", province = new { province.Id, province.Value, province.Code }, alternatives };
    }

    [McpServerTool(Name = "list_sources"), Description("Lists the candidate sources configured in Sigook (ZipRecruiter, Indeed, Other...). Use the id as sourceId in create_candidate.")]
    public static Task<List<CatalogItem>> ListSources(ISigookApi api, CancellationToken cancellationToken) => api.GetSources(cancellationToken).OrThrow();

    [McpServerTool(Name = "list_genders"), Description("Lists the gender catalog of Sigook. Use the id as genderId in create_candidate.")]
    public static Task<List<CatalogItem>> ListGenders(ISigookApi api, CancellationToken cancellationToken) => api.GetGenders(cancellationToken).OrThrow();

    [McpServerTool(Name = "list_skills"), Description("Lists the skill vocabulary Sigook already uses (worker skill catalog). Prefer these names when extracting skills from a resume so candidates stay searchable; the API still accepts new skills.")]
    public static async Task<string[]> ListSkills(ISigookApi api, CancellationToken cancellationToken) =>
        [.. (await api.GetSkills(cancellationToken).OrThrow()).Select(s => s.Skill).Where(s => !string.IsNullOrWhiteSpace(s)).Order()];

    [McpServerTool(Name = "find_candidates"), Description("Searches existing candidates of the current agency by name and/or phone. Use it before create_candidate to detect duplicates. Phone must be in Sigook format (### ###-####); use normalize_phone first.")]
    public static async Task<object> FindCandidates(
        ISigookApi api,
        [Description("Full or partial name.")] string? name = null,
        [Description("Phone in Sigook format ### ###-####.")] string? phone = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(phone))
            return new { error = "Provide name or phone." };
        var page = await api.GetCandidates(name, phone, 10, cancellationToken).OrThrow();
        return new
        {
            page.TotalItems,
            candidates = page.Items.Select(c => new
            {
                c.Id,
                c.Name,
                c.Email,
                phones = c.PhoneNumbers?.Select(p => p.PhoneNumber),
                c.City,
                c.Source,
                c.Recruiter,
                c.CreatedAt,
                c.Dnu,
                requests = c.Requests?.Select(r => r.Value)
            })
        };
    }

    [McpServerTool(Name = "normalize_phone"), Description("Converts any North American phone string (e.g. '+1 4452510680', '(445) 251-0680') into the format Sigook requires: ### ###-####. Returns an error when the number does not have 10 digits.")]
    public static object NormalizePhone([Description("Raw phone number.")] string phone)
    {
        var digits = DigitsOnly().Replace(phone ?? string.Empty, string.Empty);
        if (digits.Length == 11 && digits[0] == '1') digits = digits[1..];
        if (digits.Length != 10)
            return new { ok = false, input = phone, error = $"Expected 10 digits, got {digits.Length}." };
        return new { ok = true, input = phone, phone = $"{digits[..3]} {digits[3..6]}-{digits[6..]}" };
    }

    private static bool Same(string? a, string? b) =>
        !string.IsNullOrWhiteSpace(a) && !string.IsNullOrWhiteSpace(b) && Fold(a) == Fold(b);

    private static string Fold(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;
            if (char.IsLetterOrDigit(c)) builder.Append(char.ToLowerInvariant(c));
        }
        return builder.ToString();
    }

    private static double Similarity(string a, string b)
    {
        if (a.Length == 0 || b.Length == 0) return 0;
        if (a.Contains(b) || b.Contains(a)) return 0.9;
        var distance = Levenshtein(a, b);
        return 1.0 - (double)distance / Math.Max(a.Length, b.Length);
    }

    private static int Levenshtein(string a, string b)
    {
        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];
        for (var j = 0; j <= b.Length; j++) previous[j] = j;
        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                current[j] = Math.Min(Math.Min(current[j - 1] + 1, previous[j] + 1), previous[j - 1] + cost);
            }
            (previous, current) = (current, previous);
        }
        return previous[b.Length];
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex DigitsOnly();
}
