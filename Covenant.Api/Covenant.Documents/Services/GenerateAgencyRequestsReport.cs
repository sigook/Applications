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
        "Request ID",
        "Client",
        "Location",
        "Position",
        "Created",
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
        var location = string.IsNullOrWhiteSpace(data.Entrance) ? data.Location : $"{data.Location} - {data.Entrance}";
        var position = string.IsNullOrWhiteSpace(data.BillingTitle) ? data.JobTitle : $"{data.JobTitle} ({data.BillingTitle})";
        var created = string.IsNullOrWhiteSpace(data.DisplayShift)
            ? data.CreatedAt.ToString("yyyy-MM-dd")
            : $"{data.CreatedAt:yyyy-MM-dd} - {data.DisplayShift}";

        sheet.Cell($"A{row}").SetValue(data.NumberId);
        sheet.Cell($"B{row}").SetValue(data.CompanyFullName);
        sheet.Cell($"C{row}").SetValue(location);
        sheet.Cell($"D{row}").SetValue(position);
        sheet.Cell($"E{row}").SetValue(created);
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
