using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Subcontractor;
using Covenant.Common.Utils.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public class GenerateSubcontractorReport : IRequest<ResultGenerateDocument<byte[]>>
{
    public GenerateSubcontractorReport(IReadOnlyList<ReportSubcontractorModel> model)
    {
        Model = model;
    }

    public IReadOnlyList<ReportSubcontractorModel> Model { get; }
}

public class GenerateSubcontractorReportHandler : IRequestHandler<GenerateSubcontractorReport, ResultGenerateDocument<byte[]>>
{
    private const string No = "A",
        FullName = "B",
        Company = "C",
        Description = "D",
        Quantity = "E",
        Rate = "F",
        Total = "G",
        Deductions = "H",
        PublicHoliday = "I",
        TotalNet = "J",
        Signature = "K";

    private const int HeadRow = 11;

    private readonly IEnumerable<string> _columns = new[]
    {
        No,
        FullName,
        Company,
        Description,
        Quantity,
        Rate,
        Total,
        Deductions,
        PublicHoliday,
        TotalNet,
        Signature
    };

    private readonly XLColor CustomGreen = XLColor.FromArgb(236, 240, 223);

    public async Task<ResultGenerateDocument<byte[]>> Handle(GenerateSubcontractorReport request, CancellationToken cancellationToken)
    {
        var list = request.Model;
        using var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");
        SetUpHeader(worksheet);
        worksheet.Cell("A2").SetValue("SUBCONTRACTOR NAME").Style.Font.Bold = true;
        worksheet.Cell("A4").SetValue("CUSTOMER: COVENANT GROUP LIMITED").Style.Font.Bold = true;
        worksheet.Cell("B9").SetValue(list.First().WeekEnding.ToString("D")).Style.Font.Bold = true;
        foreach (string column in _columns) worksheet.Column(column).Width = 15;
        int rowIndex = HeadRow + 1;
        var color = XLColor.White;
        var no = 1;
        foreach (var item in list)
        {
            var items = item.ItemsGroupByWorkerRate;
            int itemsCount = items.Count;
            for (var fIndex = 0; fIndex < itemsCount; fIndex++)
            {
                var subItem = items[fIndex];
                if (subItem.Regular > decimal.Zero)
                {
                    SetBorders(worksheet, rowIndex);
                    worksheet.Cell($"{Company}{rowIndex}").SetValue(subItem.Company);
                    worksheet.Cell($"{Description}{rowIndex}").SetValue("Regular");
                    worksheet.Cell($"{Quantity}{rowIndex}").SetValue(subItem.RegularHours);
                    worksheet.Cell($"{Rate}{rowIndex}").SetValue(subItem.WorkerRate).SetMoneyType();
                    worksheet.Cell($"{Total}{rowIndex}").SetValue(subItem.Regular).SetMoneyType();
                    worksheet.Row(rowIndex).Style.Fill.BackgroundColor = color;
                    rowIndex++;
                }
                if (subItem.OtherRegular > decimal.Zero)
                {
                    SetBorders(worksheet, rowIndex);
                    worksheet.Cell($"{Company}{rowIndex}").SetValue(subItem.Company);
                    worksheet.Cell($"{Description}{rowIndex}").SetValue("Other Regular");
                    worksheet.Cell($"{Quantity}{rowIndex}").SetValue(subItem.OtherRegularHours);
                    worksheet.Cell($"{Rate}{rowIndex}").SetValue(subItem.WorkerRate).SetMoneyType();
                    worksheet.Cell($"{Total}{rowIndex}").SetValue(subItem.OtherRegular).SetMoneyType();
                    worksheet.Row(rowIndex).Style.Fill.BackgroundColor = color;
                    rowIndex++;
                }
                if (subItem.Overtime > decimal.Zero)
                {
                    SetBorders(worksheet, rowIndex);
                    worksheet.Cell($"{Company}{rowIndex}").SetValue(subItem.Company);
                    worksheet.Cell($"{Description}{rowIndex}").SetValue("Overtime");
                    worksheet.Cell($"{Quantity}{rowIndex}").SetValue(subItem.OvertimeHours);
                    SetRateFormula(worksheet.Cell($"{Rate}{rowIndex}"), rowIndex);
                    worksheet.Cell($"{Total}{rowIndex}").SetValue(subItem.Overtime).SetMoneyType();
                    worksheet.Row(rowIndex).Style.Fill.BackgroundColor = color;
                    rowIndex++;
                }
                if (subItem.Holiday > decimal.Zero)
                {
                    SetBorders(worksheet, rowIndex);
                    worksheet.Cell($"{Company}{rowIndex}").SetValue(subItem.Company);
                    worksheet.Cell($"{Description}{rowIndex}").SetValue("Premium Pay");
                    worksheet.Cell($"{Quantity}{rowIndex}").SetValue(subItem.HolidayHours);
                    SetRateFormula(worksheet.Cell($"{Rate}{rowIndex}"), rowIndex);
                    worksheet.Cell($"{Total}{rowIndex}").SetValue(subItem.Holiday).SetMoneyType();
                    worksheet.Row(rowIndex).Style.Fill.BackgroundColor = color;
                    rowIndex++;
                }
                if (subItem.Missing > decimal.Zero)
                {
                    SetBorders(worksheet, rowIndex);
                    worksheet.Cell($"{Company}{rowIndex}").SetValue(subItem.Company);
                    worksheet.Cell($"{Description}{rowIndex}").SetValue("Missing");
                    worksheet.Cell($"{Quantity}{rowIndex}").SetValue(subItem.MissingHours);
                    SetRateFormula(worksheet.Cell($"{Rate}{rowIndex}"), rowIndex);
                    worksheet.Cell($"{Total}{rowIndex}").SetValue(subItem.Missing).SetMoneyType();
                    worksheet.Row(rowIndex).Style.Fill.BackgroundColor = color;
                    rowIndex++;
                }

                if (subItem.MissingOvertime <= decimal.Zero) continue;
                SetBorders(worksheet, rowIndex);
                worksheet.Cell($"{Company}{rowIndex}").SetValue(subItem.Company);
                worksheet.Cell($"{Description}{rowIndex}").SetValue("Missing Overtime");
                worksheet.Cell($"{Quantity}{rowIndex}").SetValue(subItem.MissingOvertimeHours);
                SetRateFormula(worksheet.Cell($"{Rate}{rowIndex}"), rowIndex);
                worksheet.Cell($"{Total}{rowIndex}").SetValue(subItem.MissingOvertime).SetMoneyType();
                worksheet.Row(rowIndex).Style.Fill.BackgroundColor = color;
                rowIndex++;
            }
            rowIndex--;
            worksheet.Cell($"{No}{rowIndex}").SetValue(no);
            worksheet.Cell($"{FullName}{rowIndex}").SetValue(item.FullName);
            worksheet.Cell($"{Deductions}{rowIndex}").SetValue(item.Deductions).SetMoneyType();
            worksheet.Cell($"{PublicHoliday}{rowIndex}").SetValue(item.PublicHoliday).SetMoneyType();
            worksheet.Cell($"{TotalNet}{rowIndex}").SetValue(item.TotalNet).SetMoneyType();
            worksheet.Cell($"{Signature}{rowIndex}").SetValue(item.Email);
            worksheet.Row(rowIndex).Style.Fill.BackgroundColor = color;
            worksheet.Row(rowIndex).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rowIndex++;
            color = color == XLColor.White ? CustomGreen : XLColor.White;
            no++;
        }

        worksheet.Cell($"{Total}{rowIndex}").SetValue("TOTAL").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell($"{Deductions}{rowIndex}").SetFormulaA1($"SUM({Deductions}{HeadRow + 1}:{Deductions}{rowIndex - 1})").SetMoneyType();
        worksheet.Cell($"{PublicHoliday}{rowIndex}").SetFormulaA1($"SUM({PublicHoliday}{HeadRow + 1}:{PublicHoliday}{rowIndex - 1})").SetMoneyType();
        worksheet.Cell($"{TotalNet}{rowIndex}").SetFormulaA1($"SUM({TotalNet}{HeadRow + 1}:{TotalNet}{rowIndex - 1})").SetMoneyType();
        worksheet.Range($"{Total}{rowIndex}", $"{TotalNet}{rowIndex}").Style
            .Border.SetTopBorder(XLBorderStyleValues.Medium)
            .Border.SetRightBorder(XLBorderStyleValues.Medium)
            .Border.SetBottomBorder(XLBorderStyleValues.Medium)
            .Border.SetLeftBorder(XLBorderStyleValues.Medium);
        workbook.SaveAs(memoryStream);
        await Task.CompletedTask;
        return new ResultGenerateDocument<byte[]>(memoryStream.ToArray(), $"SubcontractorReport_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
    }

    private void SetUpHeader(IXLWorksheet worksheet)
    {
        SetBorders(worksheet, HeadRow);
        worksheet.Row(HeadRow).Style.Font.Bold = true;
        worksheet.Row(HeadRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Row(HeadRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        worksheet.Row(HeadRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
        worksheet.Cell($"{No}{HeadRow}").SetValue("NO.");
        worksheet.Cell($"{FullName}{HeadRow}").SetValue("FULL NAME");
        worksheet.Cell($"{Company}{HeadRow}").SetValue("COMPANY");
        worksheet.Cell($"{Description}{HeadRow}").SetValue("DESCRIPTION");
        worksheet.Cell($"{Quantity}{HeadRow}").SetValue("QUANTITY");
        worksheet.Cell($"{Rate}{HeadRow}").SetValue("RATE");
        worksheet.Cell($"{Total}{HeadRow}").SetValue("TOTAL");
        worksheet.Cell($"{Deductions}{HeadRow}").SetValue("DEDUCTIONS");
        worksheet.Cell($"{PublicHoliday}{HeadRow}").SetValue("PUBLIC HOLIDAY");
        worksheet.Cell($"{TotalNet}{HeadRow}").SetValue("TOTAL NET");
        worksheet.Cell($"{Signature}{HeadRow}").SetValue("SIGNATURE");
    }

    private IXLWorksheet SetBorders(IXLWorksheet worksheet, int rowNumber)
    {
        foreach (string column in _columns)
        {
            IXLCell cell = worksheet.Cell($"{column}{rowNumber}");
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }
        return worksheet;
    }

    private IXLCell SetRateFormula(IXLCell cell, int rowIndex)
    {
        cell.SetFormulaA1($"=ROUND({Total}{rowIndex}/{Quantity}{rowIndex},2)");
        return cell;
    }
}
