using ClosedXML.Excel;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Documents.Services;

public class GenerateHoursWorkedReport : GenerateAgencyReport<HoursWorkedResponse>
{
    public GenerateHoursWorkedReport(IReadOnlyList<HoursWorkedResponse> model)
        : base(model)
    {

    }

    public override IEnumerable<string> Columns => new string[]
    {
        "Worker Name",
        "Job Position",
        "Bill Rate",
        "Regular Hours",
        "Overtime Hours",
        "Holiday Hours",
        "Night Hours",
        "Total Regular Bill Rate",
        "Total Overtime Bill Rate",
        "Total Holiday Bill Rate",
        "Total Night Bill Rate",
        "Total Hours",
        "Total Bill Rate"
    };
}

public class GenerateHoursWorkedReportHandler : GenerateAgencyReportHandler<GenerateHoursWorkedReport, HoursWorkedResponse>
{
    public override void SetValue(IXLWorksheet sheet, int row, HoursWorkedResponse data)
    {
        sheet.Cell($"A{row}").SetValue(data.WorkerName).AdjustToContents();
        sheet.Cell($"B{row}").SetValue(data.JobPosition).AdjustToContents();
        sheet.Cell($"C{row}").SetValue(data.BillRate).AdjustToContents();
        sheet.Cell($"D{row}").SetValue(data.RegularHoursWorked).AdjustToContents();
        sheet.Cell($"E{row}").SetValue(data.OvertimeHoursWorked).AdjustToContents();
        sheet.Cell($"F{row}").SetValue(data.HolidayHoursWorked).AdjustToContents();
        sheet.Cell($"G{row}").SetValue(data.NightHoursWorked).AdjustToContents();
        sheet.Cell($"H{row}").SetValue(data.TotalPayRegularRate).AdjustToContents();
        sheet.Cell($"I{row}").SetValue(data.TotalPayOvertimeRate).AdjustToContents();
        sheet.Cell($"J{row}").SetValue(data.TotalPayHolidayRate).AdjustToContents();
        sheet.Cell($"K{row}").SetValue(data.TotalPayNightRate).AdjustToContents();
        sheet.Cell($"L{row}").SetValue(data.TotalHoursWorked).AdjustToContents();
        sheet.Cell($"M{row}").SetValue(data.TotalPayRate).AdjustToContents();
    }
}
