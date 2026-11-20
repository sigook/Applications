using ClosedXML.Excel;
using Covenant.Common.Models.Workers.Agencies;
using Covenant.Common.Models.Workers;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Documents.Services;

public class GenerateAgencyWorkersProfileReport(IReadOnlyList<WorkerProfileListModel> model) : GenerateAgencyReport<WorkerProfileListModel>(model)
{
    public override IEnumerable<string> Columns =>
    [
        "Number ID",
        "Full Name",
        "SIN Number",
        "Birth Date",
        "Address",
        "Phone",
        "Email",
        "Request IDs",
        "CreatedAt",
        "Skills",
        "Working",
        "DNU",
        "Approved To Work",
        "Subcontractor"
    ];
}

public class GenerateAgencyWorkersProfileReportHandler : GenerateAgencyReportHandler<GenerateAgencyWorkersProfileReport, WorkerProfileListModel>
{
    public override void SetValue(IXLWorksheet sheet, int row, WorkerProfileListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.NumberId);
        sheet.Cell($"B{row}").SetValue(data.FullName);
        sheet.Cell($"C{row}").SetValue(data.SinNumber);
        sheet.Cell($"D{row}").SetValue(data.BirthDay).Style.DateFormat.Format = "yyyy-MM-dd";
        sheet.Cell($"E{row}").SetValue(data.Address);
        sheet.Cell($"F{row}").SetValue(data.MobileNumber);
        sheet.Cell($"G{row}").SetValue(data.Email);
        sheet.Cell($"H{row}").SetValue(string.Join(", ", data.Requests.Select(r => r.Value)));
        sheet.Cell($"I{row}").SetValue(data.CreatedAt);
        sheet.Cell($"J{row}").SetValue(string.Join(", ", data.Skills));
        sheet.Cell($"K{row}").SetValue(data.IsCurrentlyWorking.YesOrNo());
        sheet.Cell($"L{row}").SetValue(data.Dnu.YesOrNo());
        sheet.Cell($"M{row}").SetValue(data.ApprovedToWork.YesOrNo());
        sheet.Cell($"N{row}").SetValue(data.IsSubcontractor.YesOrNo());
    }
}
