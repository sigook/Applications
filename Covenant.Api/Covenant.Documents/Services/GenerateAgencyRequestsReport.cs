using ClosedXML.Excel;
using Covenant.Common.Enums;
using Covenant.Common.Models.Request;

namespace Covenant.Documents.Services;

public class GenerateAgencyRequestsReport : GenerateAgencyReport<AgencyRequestListModel>
{
    public GenerateAgencyRequestsReport(IReadOnlyList<AgencyRequestListModel> model)
        : base(model)
    {
    }

    public override IEnumerable<string> Columns => new string[]
    {
        "Number ID",
        "Client",
        "Position",
        "Last Update",
        "Duration (Start - Finish)",
        "Recruiter",
        "Sales Representative",
        "Rate / Salary",
        "Workers",
        "Status"
    };
}

public class GenerateAgencyRequestReportHandler : GenerateAgencyReportHandler<GenerateAgencyRequestsReport, AgencyRequestListModel>
{
    public override void SetValue(IXLWorksheet sheet, int row, AgencyRequestListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.NumberId);
        sheet.Cell($"B{row}").SetValue(data.CompanyFullName);
        sheet.Cell($"C{row}").SetValue(data.JobTitle);
        sheet.Cell($"D{row}").SetValue(data.UpdatedAt);
        sheet.Cell($"E{row}").SetValue($"{data.StartAt} - {data.FinishAt}");
        sheet.Cell($"F{row}").SetValue(data.DisplayRecruiters);
        sheet.Cell($"G{row}").SetValue(data.SalesRepresentative);
        sheet.Cell($"H{row}").SetValue(data.WorkerRate);
        sheet.Cell($"I{row}").SetValue($"{data.WorkersQuantityWorking} / {data.WorkersQuantity}");

        var statusText = data.RequestStatus switch
        {
            RequestStatus.Open => "Open",
            RequestStatus.Filled => "Filled",
            RequestStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };
        sheet.Cell($"J{row}").SetValue(statusText);
    }
}
