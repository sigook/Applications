using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.TimeSheet;
using MediatR;

namespace Covenant.Documents.Services;

public class GenerateRequestTimeSheetReport : IRequest<ResultGenerateDocument<MemoryStream>>
{
    public GenerateRequestTimeSheetReport(IReadOnlyList<RequestTimeSheetModel> model)
    {
        Model = model;
    }

    public IReadOnlyList<RequestTimeSheetModel> Model { get; }
}

public class GenerateRequestTimeSheetDocumentHandler : IRequestHandler<GenerateRequestTimeSheetReport, ResultGenerateDocument<MemoryStream>>
{
    private const string
        Worker = "Worker",
        Date = "Date",
        ClockIn = "Clock In",
        ClockOut = "Clock Out",
        TotalHoursApproved = "Total Hours",
        Comments = "Comments";

    private static readonly string[] Columns = { Worker, Date, ClockIn, ClockOut, TotalHoursApproved, Comments };

    public string StartName { get; set; } = "TimeSheet";
    public uint StartSheetDataIndex { get; set; } = 3;

    public async Task<ResultGenerateDocument<MemoryStream>> Handle(GenerateRequestTimeSheetReport request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(StartName);
        worksheet.Cell("A1").Value = "WORKER";
        worksheet.Cell("B1").Value = $"Timesheet - {request.Model.FirstOrDefault()?.RequestTitle ?? ""} - {request.Model.FirstOrDefault()?.CompanyFullName ?? ""}";
        var headerRange = worksheet.Range("A1:F1");
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#50BAF9");
        headerRange.Style.Font.FontSize = 12;
        headerRange.Style.Font.Bold = true;
        worksheet.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromHtml("#50BAF9");
        worksheet.Cell("B2").Value = "Date";
        worksheet.Cell("C2").Value = "Clock In Reported";
        worksheet.Cell("D2").Value = "Clock Out Reported";
        worksheet.Cell("E2").Value = "Total Hours Approved";
        worksheet.Cell("F2").Value = "Comments";
        var columnHeadersRange = worksheet.Range("B2:F2");
        columnHeadersRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#BDBDBD");
        columnHeadersRange.Style.Font.Bold = true;
        columnHeadersRange.Style.Font.FontSize = 11;

        int index = (int)StartSheetDataIndex;
        foreach (var total in request.Model)
        {
            var dataRowRange = worksheet.Range($"A{index}:F{index}");
            dataRowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFEFEF");
            dataRowRange.Style.Font.FontSize = 10;

            worksheet.Cell(index, 1).Value = total.WorkerFullName;
            worksheet.Cell(index, 2).Value = total.Date.ToLongDateString();
            worksheet.Cell(index, 3).Value = total.ClockIn.HasValue ? total.ClockIn.Value.ToString("HH:mm") : string.Empty;
            worksheet.Cell(index, 4).Value = total.ClockOut.HasValue ? total.ClockOut.Value.ToString("HH:mm") : string.Empty;
            worksheet.Cell(index, 5).Value = total.TotalHours;
            worksheet.Cell(index, 6).Value = total.Comment;
            index++;
        }
        var totalRow = index + 1;
        var totalRowRange = worksheet.Range($"A{totalRow}:F{totalRow}");
        totalRowRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFEFEF");
        totalRowRange.Style.Font.Bold = true;
        totalRowRange.Style.Font.FontSize = 11;

        worksheet.Cell(totalRow, 1).Value = "TOTAL";
        worksheet.Cell(totalRow, 5).FormulaA1 = $"SUM(E{StartSheetDataIndex}:E{index - 1})";
        worksheet.Column("A").Width = 25;
        worksheet.Column("B").Width = 20;
        worksheet.Column("C").Width = 18;
        worksheet.Column("D").Width = 18;
        worksheet.Column("E").Width = 20;
        worksheet.Column("F").Width = 25;

        workbook.SaveAs(memoryStream);
        var documentName = $"{StartName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        await Task.CompletedTask;
        return new ResultGenerateDocument<MemoryStream>(memoryStream, documentName, string.Empty);
    }
}
