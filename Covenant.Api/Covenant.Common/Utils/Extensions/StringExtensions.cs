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

    public static string NormalizeForComparison(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        return builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }

    private static string ToAccountingFileBlobName(string accountingFileType, Guid id) => $"{accountingFileType}_{id:N}.pdf";
}
