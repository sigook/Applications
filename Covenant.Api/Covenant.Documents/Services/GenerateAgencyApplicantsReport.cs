using ClosedXML.Excel;
using Covenant.Common.Enums;
using Covenant.Common.Models.Request;

namespace Covenant.Documents.Services;

public class GenerateAgencyApplicantsReport(IReadOnlyList<AgencyApplicantListModel> model)
    : GenerateAgencyReport<AgencyApplicantListModel>(model)
{
    public override IEnumerable<string> Columns =>
    [
        "Request",
        "Client",
        "Position",
        "Starts",
        "Applicant",
        "Email",
        "Phone",
        "Type",
        "Status",
        "Compliance",
        "Mandatory Pending",
        "Added By",
        "Added At",
        "Comments",
    ];
}

public class GenerateAgencyApplicantsReportHandler : GenerateAgencyReportHandler<GenerateAgencyApplicantsReport, AgencyApplicantListModel>
{
    public override void SetValue(IXLWorksheet sheet, int row, AgencyApplicantListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.RequestNumberId);
        sheet.Cell($"B{row}").SetValue(data.CompanyFullName);
        sheet.Cell($"C{row}").SetValue(data.JobTitle);
        sheet.Cell($"D{row}").SetValue(data.StartAt);
        sheet.Cell($"E{row}").SetValue(data.Name);
        sheet.Cell($"F{row}").SetValue(data.Email);
        sheet.Cell($"G{row}").SetValue(data.PhoneNumber);
        sheet.Cell($"H{row}").SetValue(data.CandidateId.HasValue ? "Candidate" : "Worker");
        sheet.Cell($"I{row}").SetValue(StatusLabel(data.Status));
        sheet.Cell($"J{row}").SetValue($"{data.ComplianceCompleted}/{data.ComplianceTotal}");
        sheet.Cell($"K{row}").SetValue(data.MandatoryPending);
        sheet.Cell($"L{row}").SetValue(data.CreatedBy);
        sheet.Cell($"M{row}").SetValue(data.CreatedAt);
        sheet.Cell($"N{row}").SetValue(data.Comments);
    }

    private static string StatusLabel(RequestApplicantStatus status) => status switch
    {
        RequestApplicantStatus.Pending => "Pending",
        RequestApplicantStatus.InProgress => "In progress",
        RequestApplicantStatus.Confirmed => "Confirmed",
        _ => "Cancelled"
    };
}
