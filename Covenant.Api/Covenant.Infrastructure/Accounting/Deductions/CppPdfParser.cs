using System.Globalization;
using System.Text.RegularExpressions;
using Covenant.Common.Interfaces.Accounting;
using Covenant.Common.Models.Accounting.Deductions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Covenant.Infrastructure.Accounting.Deductions;

/// <summary>
/// Reads the CPP contribution tables from the payroll deductions PDF published by the CRA
/// https://www.canada.ca/en/revenue-agency/services/tax/businesses/topics/payroll/t4032-payroll-deductions-tables.html
/// Each printed line holds four side by side blocks of "From - To  CPP".
/// </summary>
public partial class CppPdfParser : ICppPdfParser
{
    private const double LineTolerance = 2d;

    public IReadOnlyList<CppRow> Parse(Stream pdf)
    {
        var rows = new List<CppRow>();
        using var document = PdfDocument.Open(pdf);
        foreach (var page in document.GetPages())
        {
            foreach (var line in GetLines(page))
            {
                foreach (Match match in RangeRow().Matches(line))
                {
                    rows.Add(new CppRow(ToDecimal(match.Groups[1].Value), ToDecimal(match.Groups[2].Value), ToDecimal(match.Groups[3].Value)));
                }
            }
        }
        return rows;
    }

    private static IEnumerable<string> GetLines(Page page)
    {
        var current = new List<Word>();
        var baseline = 0d;
        foreach (var word in page.GetWords().OrderByDescending(w => w.BoundingBox.Bottom))
        {
            if (current.Count > 0 && Math.Abs(baseline - word.BoundingBox.Bottom) > LineTolerance)
            {
                yield return Join(current);
                current.Clear();
            }
            if (current.Count == 0)
            {
                baseline = word.BoundingBox.Bottom;
            }
            current.Add(word);
        }
        if (current.Count > 0)
        {
            yield return Join(current);
        }
    }

    private static string Join(IEnumerable<Word> words) =>
        string.Join(" ", words.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text));

    private static decimal ToDecimal(string value) => decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);

    [GeneratedRegex(@"(\d*\.\d{2})\s*-\s*(\d*\.\d{2})\s+(?:\(\d+\)\s*)?(\d*\.\d{2})")]
    private static partial Regex RangeRow();
}
