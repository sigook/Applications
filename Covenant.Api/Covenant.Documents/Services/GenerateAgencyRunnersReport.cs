using ClosedXML.Excel;
using Covenant.Common.Enums;
using Covenant.Common.Models.Requests.Runners;

namespace Covenant.Documents.Services;

public class GenerateAgencyRunnersReport(IReadOnlyList<AgencyRunnerListModel> model)
    : GenerateAgencyReport<AgencyRunnerListModel>(model)
{
    public override IEnumerable<string> Columns =>
    [
        "Request",
        "Client",
        "Position",
        "Starts",
        "Runner",
        "Email",
        "Type",
        "Status",
        "Interviews",
        "Last Interview",
        "Start Date",
        "Added By",
        "Added At",
    ];
}

public class GenerateAgencyRunnersReportHandler : GenerateAgencyReportHandler<GenerateAgencyRunnersReport, AgencyRunnerListModel>
{
    public override void SetValue(IXLWorksheet sheet, int row, AgencyRunnerListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.RequestNumberId);
        sheet.Cell($"B{row}").SetValue(data.CompanyFullName);
        sheet.Cell($"C{row}").SetValue(data.JobTitle);
        sheet.Cell($"D{row}").SetValue(data.StartAt);
        sheet.Cell($"E{row}").SetValue(data.Name);
        sheet.Cell($"F{row}").SetValue(data.Email);
        sheet.Cell($"G{row}").SetValue(data.Type == RunnerType.Active ? "Active" : "Passive");
        sheet.Cell($"H{row}").SetValue(StatusLabel(data.Status));
        sheet.Cell($"I{row}").SetValue(data.InterviewsCount);
        sheet.Cell($"J{row}").SetValue(data.LastInterviewAt);
        sheet.Cell($"K{row}").SetValue(data.StartDate);
        sheet.Cell($"L{row}").SetValue(data.CreatedByEmail);
        sheet.Cell($"M{row}").SetValue(data.CreatedAt);
    }

    private static string StatusLabel(RunnerStatus status) => status switch
    {
        RunnerStatus.SentToClient => "Sent to Client",
        RunnerStatus.InterviewScheduled => "Interview scheduled",
        RunnerStatus.InterviewRescheduled => "Interview rescheduled",
        RunnerStatus.NoLongerAvailable => "No longer available",
        RunnerStatus.NoShow => "No show",
        RunnerStatus.WaitingForInterviewFeedback => "Waiting for interview feedback",
        RunnerStatus.WaitingForFinalDecision => "Waiting for final decision",
        RunnerStatus.Rejected => "Rejected",
        RunnerStatus.InOnboardingProcess => "In onboarding process",
        RunnerStatus.Hired => "Hired",
        _ => status.ToString()
    };
}
