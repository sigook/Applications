using System.Text.RegularExpressions;
using Sigook.Functions.Models;

namespace Sigook.Functions.Utils;

/// <summary>
/// Reads the table, the pay period and the year from the name of a CRA table uploaded to the cra-tables container.
/// </summary>
public static partial class CraBlobName
{
    public const string Convention = "<CPP|TAX> <WEEKLY|BIWEEKLY|SEMIMONTHLY|MONTHLY> <YYYY>.pdf";
    private const string PdfExtension = ".pdf";
    private const int MinimumYear = 2000;
    private const int MaximumYear = 2100;

    public static bool TryParse(string blobName, out CraTable table, out string error)
    {
        table = null;
        if (string.IsNullOrWhiteSpace(blobName))
        {
            error = "The blob name is empty";
            return false;
        }
        if (!blobName.EndsWith(PdfExtension, StringComparison.InvariantCultureIgnoreCase))
        {
            error = $"'{blobName}' is not a PDF file";
            return false;
        }
        var match = CraTableName().Match(blobName);
        if (!match.Success)
        {
            error = $"'{blobName}' does not follow the convention {Convention}";
            return false;
        }
        var year = int.Parse(match.Groups["year"].Value);
        if (year < MinimumYear || year > MaximumYear)
        {
            error = $"The year {year} is outside the supported range {MinimumYear}-{MaximumYear}";
            return false;
        }

        table = new CraTable(ToKind(match.Groups["table"].Value), new ImportCraTableFromBlobModel
        {
            BlobName = blobName,
            PayPeriod = ToPayPeriod(match.Groups["period"].Value),
            Year = year
        });
        error = null;
        return true;
    }

    private static CraTableKind ToKind(string value) =>
        value.ToUpperInvariant() == "CPP" ? CraTableKind.Cpp : CraTableKind.Tax;

    private static PayPeriod ToPayPeriod(string value) =>
        new string(value.Where(char.IsLetter).ToArray()).ToUpperInvariant() switch
        {
            "WEEKLY" => PayPeriod.Weekly,
            "BIWEEKLY" => PayPeriod.BiWeekly,
            "SEMIMONTHLY" => PayPeriod.SemiMonthly,
            _ => PayPeriod.Monthly
        };

    [GeneratedRegex(@"^(?<table>CPP|TAX)[ _-]+(?<period>WEEKLY|BI[ _-]?WEEKLY|SEMI[ _-]?MONTHLY|MONTHLY)[ _-]+(?<year>\d{4})\.pdf$",
        RegexOptions.IgnoreCase)]
    private static partial Regex CraTableName();
}
