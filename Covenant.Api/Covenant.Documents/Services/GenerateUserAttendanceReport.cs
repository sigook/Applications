using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using MediatR;

namespace Covenant.Documents.Services;

public class GenerateUserAttendanceReport(UserAttendanceReportModel model) : IRequest<ResultGenerateDocument<MemoryStream>>
{
    public UserAttendanceReportModel Model { get; } = model;
}

public class GenerateUserAttendanceReportHandler : IRequestHandler<GenerateUserAttendanceReport, ResultGenerateDocument<MemoryStream>>
{
    private const string SheetName = "Attendance";
    private static readonly string[] Columns =
        ["User", "Date", "Clock In", "Clock Out", "Worked", "Lunch", "Regular", "Overtime", "Status", "Edited By", "Edit Reason"];

    public Task<ResultGenerateDocument<MemoryStream>> Handle(GenerateUserAttendanceReport request, CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(SheetName);

        for (int column = 0; column < Columns.Length; column++)
            worksheet.Cell(1, column + 1).Value = Columns[column];
        var header = worksheet.Range(1, 1, 1, Columns.Length);
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#50BAF9");
        header.Style.Font.Bold = true;

        int row = 2;
        foreach (var item in request.Model.Items)
        {
            worksheet.Cell(row, 1).Value = item.Name;
            worksheet.Cell(row, 2).Value = item.Date;
            worksheet.Cell(row, 2).Style.DateFormat.Format = "ddd, MMM d, yyyy";
            worksheet.Cell(row, 3).Value = item.ClockIn.ToString("HH:mm");
            worksheet.Cell(row, 4).Value = item.ClockOut?.ToString("HH:mm") ?? string.Empty;
            worksheet.Cell(row, 5).Value = item.WorkedHours;
            worksheet.Cell(row, 6).Value = item.LunchHours;
            worksheet.Cell(row, 7).Value = item.RegularHours;
            worksheet.Cell(row, 8).Value = item.OvertimeHours;
            worksheet.Cell(row, 9).Value = Status(item);
            worksheet.Cell(row, 10).Value = item.EditedBy ?? string.Empty;
            worksheet.Cell(row, 11).Value = item.EditReason ?? string.Empty;
            row++;
        }

        var totals = request.Model.Totals;
        worksheet.Cell(row, 1).Value = "TOTAL";
        worksheet.Cell(row, 5).Value = totals.WorkedHours;
        worksheet.Cell(row, 6).Value = totals.LunchHours;
        worksheet.Cell(row, 7).Value = totals.RegularHours;
        worksheet.Cell(row, 8).Value = totals.OvertimeHours;
        worksheet.Range(row, 1, row, Columns.Length).Style.Font.Bold = true;
        worksheet.Range(2, 5, row, 8).Style.NumberFormat.Format = "0.00";
        worksheet.Column(8).Style.Font.FontColor = XLColor.FromHtml("#B35C00");
        worksheet.Columns().AdjustToContents();

        workbook.SaveAs(memoryStream);
        memoryStream.Position = 0;
        var documentName = $"{SheetName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return Task.FromResult(new ResultGenerateDocument<MemoryStream>(memoryStream, documentName, string.Empty));
    }

    private static string Status(UserAttendanceListModel item)
    {
        if (item.IsMissingClockOut) return "Missing clock out";
        var tags = new List<string>();
        if (item.IsWeekend) tags.Add("Weekend");
        if (item.IsEdited) tags.Add("Edited");
        return string.Join(", ", tags);
    }
}
