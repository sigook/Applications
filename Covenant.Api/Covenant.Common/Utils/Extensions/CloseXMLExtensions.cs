using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Covenant.Common.Utils.Extensions;

public static class CloseXMLExtensions
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

    public static Cell GetCell(this SheetData sheetData, string cellReference) =>
        sheetData.Descendants<Cell>().FirstOrDefault(c => c.CellReference == cellReference);

    public static Row GetRow(this SheetData sheetData, uint rowIndex)
    {
        Row row = sheetData.Elements<Row>().FirstOrDefault(c => c.RowIndex == rowIndex);
        return row ?? sheetData.AppendChild(new Row { RowIndex = rowIndex });
    }

    public static Cell AddCell2(this Row row, string columnReference, Cell cellToClone)
    {
        string cellReference = $"{columnReference}{row.RowIndex}";
        Cell cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference == cellReference);
        if (cell != null) return cell;
        cell = (Cell)cellToClone.CloneNode(true);
        cell.CellReference = cellReference;
        row.AppendChild(cell);
        return cell;
    }

    public static Cell AddValue(this Cell cell, string value)
    {
        cell.DataType = CellValues.String;
        if (cell.CellValue is null) cell.CellValue = new CellValue();
        cell.CellValue.Text = value;
        cell.CellFormula?.ClearAllAttributes();
        return cell;
    }

    public static Cell AddFormula(this Cell cell, string formula)
    {
        cell.DataType = CellValues.Number;
        cell.CellValue = new CellValue();
        if (cell.CellFormula is null) cell.CellFormula = new CellFormula();
        cell.CellFormula.Text = formula;
        return cell;
    }
}
