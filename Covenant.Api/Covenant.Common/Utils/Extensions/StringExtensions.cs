using System.Text;
using System.Globalization;

namespace Covenant.Common.Utils.Extensions;

public static class StringExtensions
{
    public static string ToInvoiceBlobName(this Guid id) => ToAccountingFileBlobName("Invoice", id);

    public static string ToPayStubBlobName(this Guid id) => ToAccountingFileBlobName("PayStub", id);

    public static bool IsValidLength(this string value, int min, int max)
    {
        int length = value?.Length ?? 0;
        return length >= min && length <= max;
    }

    public static string MaskSIN(this string sin)
    {
        if (string.IsNullOrEmpty(sin)) return string.Empty;
        var lastFour = sin.Length <= 4 ? sin : sin.Substring(sin.Length - 4);
        return "******" + lastFour;
    }

    public static string RemoveDiacritics(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) builder.Append(c);
        }
        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static bool ContainsNormalized(this string source, string value)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(value)) return false;
        return source.RemoveDiacritics().IndexOf(value.RemoveDiacritics(), StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static string CollapseWhitespace(this string value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : string.Join(' ', value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));

    public static string ToComparisonKey(this string value) => value.RemoveDiacritics().CollapseWhitespace().ToLowerInvariant();

    public static string[] ToSearchTokens(this string term) =>
        string.IsNullOrWhiteSpace(term)
            ? []
            : term.RemoveDiacritics().ToLowerInvariant()
                .Split((char[])null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToArray();

    public static bool MatchesSearch(this string term, params string[] fields)
    {
        var tokens = term.ToSearchTokens();
        if (tokens.Length == 0) return true;
        var values = fields
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .Select(f => f.RemoveDiacritics().ToLowerInvariant())
            .ToArray();
        return tokens.All(token => values.Any(value => value.Contains(token, StringComparison.Ordinal)));
    }

    private static string ToAccountingFileBlobName(string accountingFileType, Guid id) => $"{accountingFileType}_{id:N}.pdf";
}
