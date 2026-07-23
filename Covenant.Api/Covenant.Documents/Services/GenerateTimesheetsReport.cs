using ClosedXML.Excel;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Documents.Services;

public class GenerateTimesheetsReport : GenerateAgencyReport<TimesheetsReportResponse>
{
    public GenerateTimesheetsReport(IReadOnlyList<TimesheetsReportResponse> model)
        : base(model)
    {

    }

    public override IEnumerable<string> Columns => new string[]
    {
        "#",
        "Employee ID",
        "Full Name Employee",
        "SSN",
        "WC Code",
        "Client",
        "Job Name",
        "Pay Rate",
        "Reg. Hours",
        "Overtime"
    };
}

public class GenerateTimesheetsReportHandler : GenerateAgencyReportHandler<GenerateTimesheetsReport, TimesheetsReportResponse>
{
    public override void SetValue(IXLWorksheet sheet, int row, TimesheetsReportResponse data)
    {
        sheet.Cell($"A{row}").SetValue(row - 1).AdjustToContents();
        sheet.Cell($"B{row}").SetValue(data.EmployeeId).AdjustToContents();
        sheet.Cell($"C{row}").SetValue(data.FullName).AdjustToContents();
        sheet.Cell($"D{row}").SetValue(data.SocialInsurance).AdjustToContents();
        sheet.Cell($"E{row}").SetValue(data.WcCode).AdjustToContents();
        sheet.Cell($"F{row}").SetValue(data.Client).AdjustToContents();
        sheet.Cell($"G{row}").SetValue(data.JobName).AdjustToContents();
        sheet.Cell($"H{row}").SetValue(data.PayRate).AdjustToContents();
        sheet.Cell($"I{row}").SetValue(data.RegularHours).AdjustToContents();
        sheet.Cell($"J{row}").SetValue(data.OvertimeHours).AdjustToContents();
    }
}
