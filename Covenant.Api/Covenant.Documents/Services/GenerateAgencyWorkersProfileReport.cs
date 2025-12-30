using ClosedXML.Excel;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Documents.Services
{
    public class GenerateAgencyWorkersProfileReport : GenerateAgencyReport<WorkerProfileListModel>
    {
        public GenerateAgencyWorkersProfileReport(IReadOnlyList<WorkerProfileListModel> model)
            : base(model)
        {
        }

        public override IEnumerable<string> Columns => new string[]
        {
            "Number ID",
            "Full Name",
            "SIN Number",
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
        };
    }

    public class GenerateAgencyWorkersProfileReportHandler : GenerateAgencyReportHandler<GenerateAgencyWorkersProfileReport, WorkerProfileListModel>
    {
        public override void SetValue(IXLWorksheet sheet, int row, WorkerProfileListModel data)
        {
            sheet.Cell($"A{row}").SetValue(data.NumberId);
            sheet.Cell($"B{row}").SetValue(data.FullName);
            sheet.Cell($"C{row}").SetValue(data.SinNumber);
            sheet.Cell($"D{row}").SetValue(data.Address);
            sheet.Cell($"E{row}").SetValue(data.MobileNumber);
            sheet.Cell($"F{row}").SetValue(data.Email);
            sheet.Cell($"G{row}").SetValue(string.Join(", ", data.Requests.Select(r => r.Value)));
            sheet.Cell($"H{row}").SetValue(data.CreatedAt);
            sheet.Cell($"I{row}").SetValue(string.Join(", ", data.Skills));
            sheet.Cell($"J{row}").SetValue(data.IsCurrentlyWorking.YesOrNo());
            sheet.Cell($"K{row}").SetValue(data.Dnu.YesOrNo());
            sheet.Cell($"L{row}").SetValue(data.ApprovedToWork.YesOrNo());
            sheet.Cell($"M{row}").SetValue(data.IsSubcontractor.YesOrNo());
        }
    }
}
