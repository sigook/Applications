using System.Text;

namespace Covenant.Common.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string ToLettersAndDigits(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            int length = input.Length;
            var cleanedInput = new StringBuilder(length);
            for (var i = 0; i < length; ++i)
            {
                char currentChar = input[i];
                bool charIsValid = char.IsLetterOrDigit(currentChar) || char.IsWhiteSpace(currentChar);
                if (charIsValid) cleanedInput.Append(currentChar);
            }
            return cleanedInput.ToString();
        }

        public static string GetTextSearch(this string filterText) => filterText?.Split(new string[] { " " }, StringSplitOptions.None)?.GetTextSearch();

        public static string GetTextSearch(this IEnumerable<string> items)
        {
            if (items is null) return null;
            string textSearch = string.Join("|", items.Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Contains("@") || s.Contains("-") ? s : s.ToLettersAndDigits().Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => $"'{s}':*"));
            return textSearch;
        }

        public static string ToInvoiceBlobName(this Guid id) => ToAccountingFileBlobName("Invoice", id);

        public static string ToPayStubBlobName(this Guid id) => ToAccountingFileBlobName("PayStub", id);

        private static string ToAccountingFileBlobName(string accountingFileType, Guid id) => $"{accountingFileType}_{id:N}.pdf";

        public static bool IsValidLength(this string value, int min, int max)
        {
            int length = value?.Length ?? 0;
            return length >= min && length <= max;
        }
    }
}
