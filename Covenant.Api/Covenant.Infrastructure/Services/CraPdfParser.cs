using System.Globalization;
using System.Text.RegularExpressions;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Accounting;
using Covenant.Common.Models.Accounting.Deductions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Covenant.Infrastructure.Services;

/// <summary>
/// Reads the payroll deduction tables published by the CRA
/// https://www.canada.ca/en/revenue-agency/services/tax/businesses/topics/payroll/t4032-payroll-deductions-tables.html
/// The CPP file prints four side by side blocks of "From - To  CPP" per line.
/// The income tax file prints one page per tax type, a "CC 0" to "CC 10" header and one line per pay bracket,
/// with the amounts right aligned under each claim code and no amount at all where nothing is withheld.
/// </summary>
public partial class CraPdfParser : ICraPdfParser
{
    private const double LineTolerance = 2d;
    private const string FederalTitle = "federal tax deductions";
    private const string ProvincialTitle = "provincial tax deductions";

    public IReadOnlyList<CppRow> ParseCpp(Stream pdf)
    {
        var rows = new List<CppRow>();
        using var document = PdfDocument.Open(pdf);
        foreach (var page in document.GetPages())
        {
            foreach (var line in GetLines(page))
            {
                foreach (Match match in CppBracket().Matches(Join(line)))
                {
                    rows.Add(new CppRow(ToDecimal(match.Groups[1].Value), ToDecimal(match.Groups[2].Value), ToDecimal(match.Groups[3].Value)));
                }
            }
        }
        return rows;
    }

    public IReadOnlyList<TaxRow> ParseTax(Stream pdf)
    {
        var rows = new List<TaxRow>();
        using var document = PdfDocument.Open(pdf);
        foreach (var page in document.GetPages())
        {
            var lines = GetLines(page);
            var taxType = GetTaxType(lines);
            var header = GetHeader(lines);
            if (taxType is null || header is null)
            {
                continue;
            }
            for (var index = header.Line + 1; index < lines.Count; index++)
            {
                var row = ReadTaxRow(lines[index], taxType.Value, header);
                if (row is not null)
                {
                    rows.Add(row);
                }
            }
        }
        return rows;
    }

    private static TaxRow ReadTaxRow(IReadOnlyList<Word> line, TaxType taxType, TaxHeader header)
    {
        var match = TaxBracket().Match(Join(line.Where(w => w.BoundingBox.Right < header.AmountsFrom)));
        if (!match.Success)
        {
            return null;
        }
        var from = match.Groups[2].Success ? ToDecimal(match.Groups[1].Value) : decimal.Zero;
        var to = ToDecimal(match.Groups[2].Success ? match.Groups[2].Value : match.Groups[1].Value);

        var claimCodes = new decimal?[TaxRow.ClaimCodeCount];
        foreach (var word in line.Where(w => w.BoundingBox.Right >= header.AmountsFrom && Amount().IsMatch(w.Text)))
        {
            var claimCode = header.ClaimCodeOf(word.BoundingBox.Right);
            if (claimCode >= 0)
            {
                claimCodes[claimCode] = ToDecimal(word.Text);
            }
        }
        return new TaxRow(taxType, from, to, claimCodes);
    }

    private static TaxType? GetTaxType(IReadOnlyList<IReadOnlyList<Word>> lines)
    {
        foreach (var line in lines)
        {
            var text = Join(line).ToLowerInvariant();
            if (text.Contains(FederalTitle))
            {
                return TaxType.Federal;
            }
            if (text.Contains(ProvincialTitle))
            {
                return TaxType.Provincial;
            }
        }
        return null;
    }

    private static TaxHeader GetHeader(IReadOnlyList<IReadOnlyList<Word>> lines)
    {
        for (var index = 0; index < lines.Count; index++)
        {
            var columns = GetColumns(lines[index]);
            if (columns.Count == TaxRow.ClaimCodeCount)
            {
                return new TaxHeader(index, columns);
            }
        }
        return null;
    }

    private static List<double> GetColumns(IReadOnlyList<Word> line)
    {
        var columns = new List<double>();
        for (var index = 0; index < line.Count; index++)
        {
            var match = ClaimCode().Match(line[index].Text);
            if (!match.Success)
            {
                continue;
            }
            if (match.Groups[1].Success)
            {
                columns.Add(line[index].BoundingBox.Right);
            }
            else if (index + 1 < line.Count && ClaimCodeNumber().IsMatch(line[index + 1].Text))
            {
                columns.Add(line[++index].BoundingBox.Right);
            }
        }
        return columns;
    }

    private static IReadOnlyList<IReadOnlyList<Word>> GetLines(Page page)
    {
        var lines = new List<IReadOnlyList<Word>>();
        var current = new List<Word>();
        var baseline = 0d;
        foreach (var word in page.GetWords().OrderByDescending(w => w.BoundingBox.Bottom))
        {
            if (current.Count > 0 && Math.Abs(baseline - word.BoundingBox.Bottom) > LineTolerance)
            {
                lines.Add(Sort(current));
                current = [];
            }
            if (current.Count == 0)
            {
                baseline = word.BoundingBox.Bottom;
            }
            current.Add(word);
        }
        if (current.Count > 0)
        {
            lines.Add(Sort(current));
        }
        return lines;
    }

    private static IReadOnlyList<Word> Sort(IEnumerable<Word> words) => [.. words.OrderBy(w => w.BoundingBox.Left)];

    private static string Join(IEnumerable<Word> words) =>
        string.Join(" ", words.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text));

    private static decimal ToDecimal(string value) => decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);

    /// <summary>
    /// The claim code columns of a page, keyed by the right edge of each "CC n" heading.
    /// The amounts are right aligned a few points further right than the heading, so the closest column wins
    /// as long as it stays within half the distance between two columns.
    /// </summary>
    private sealed class TaxHeader(int line, IReadOnlyList<double> columns)
    {
        private readonly double _tolerance = (columns[1] - columns[0]) / 2;

        public int Line { get; } = line;

        public double AmountsFrom { get; } = columns[0] - (columns[1] - columns[0]) / 2;

        public int ClaimCodeOf(double right)
        {
            var claimCode = -1;
            var closest = _tolerance;
            for (var index = 0; index < columns.Count; index++)
            {
                var distance = Math.Abs(columns[index] - right);
                if (distance <= closest)
                {
                    closest = distance;
                    claimCode = index;
                }
            }
            return claimCode;
        }
    }

    [GeneratedRegex(@"(\d*\.\d{2})\s*-\s*(\d*\.\d{2})\s+(?:\(\d+\)\s*)?(\d*\.\d{2})")]
    private static partial Regex CppBracket();

    [GeneratedRegex(@"^(\d+)(?:\s*-\s*(\d+))?$")]
    private static partial Regex TaxBracket();

    [GeneratedRegex(@"^\d*(?:,\d{3})*\.\d{2}$")]
    private static partial Regex Amount();

    [GeneratedRegex(@"^CC\s*(\d{1,2})?$")]
    private static partial Regex ClaimCode();

    [GeneratedRegex(@"^\d{1,2}$")]
    private static partial Regex ClaimCodeNumber();
}
