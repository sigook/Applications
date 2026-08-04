using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Covenant.Documents.Extensions;

public static class ClosedXmlExtensions
{
    private const string MoneyFormat = "$0.00";

    public static void AdjustToContents(this IXLCell cell)
    {
        var cellsUsed = cell.WorksheetColumn().CellsUsed();
        var maxLength = cellsUsed.Max(c => c.Value.ToString().Length);
        cell.WorksheetColumn().Width = maxLength;
    }

    public static IXLCell Config(this IXLCell cell, Action<IXLCell> options)
    {
        options(cell);
        return cell;
    }

    public static IXLCell SetMoneyType(this IXLCell cell)
    {
        cell.Style.NumberFormat.Format = MoneyFormat;
        return cell;
    }

    public static void SetupHeaders(this IXLWorksheet sheet, string[] headers)
    {
        var headerRow = sheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        for (int i = 0; i < headers.Length; i++)
        {
            var columnLetter = (char)(i + 65);
            sheet.Cell($"{columnLetter}1").SetValue(headers[i]);
        }
    }
}
