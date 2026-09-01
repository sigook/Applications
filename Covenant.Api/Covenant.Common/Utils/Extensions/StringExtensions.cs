using System.Globalization;
using System.Text;

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

    private static string ToAccountingFileBlobName(string accountingFileType, Guid id) => $"{accountingFileType}_{id:N}.pdf";
}
