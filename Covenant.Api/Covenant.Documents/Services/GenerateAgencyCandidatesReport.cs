using ClosedXML.Excel;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Documents.Services
{
    public class GenerateAgencyCandidatesReport : GenerateAgencyReport<CandidateListModel>
    {
        public GenerateAgencyCandidatesReport(IReadOnlyList<CandidateListModel> model)
            : base(model)
        {
        }

        public override IEnumerable<string> Columns => new string[]
        {
            "Name",
            "Email",
            "Address",
            "Postal Code",
            "Source",
            "Phone Numbers",
            "Skills",
            "Recruiter",
            "Created At",
            "Has Vehicle?",
        };
    }

    public class GenerateAgencyCandidatesReportHandler : GenerateAgencyReportHandler<GenerateAgencyCandidatesReport, CandidateListModel>
    {
        public override void SetValue(IXLWorksheet sheet, int row, CandidateListModel data)
        {
            sheet.Cell($"A{row}").SetValue(data.Name);
            sheet.Cell($"B{row}").SetValue(data.Email);
            sheet.Cell($"C{row}").SetValue(data.Address);
            sheet.Cell($"D{row}").SetValue(data.PostalCode);
            sheet.Cell($"E{row}").SetValue(data.Source);
            sheet.Cell($"F{row}").SetValue(string.Join('|', data.PhoneNumbers.Select(pn => pn.PhoneNumber)));
            sheet.Cell($"G{row}").SetValue(string.Join('|', data.Skills.Select(s => s.Skill)));
            sheet.Cell($"H{row}").SetValue(data.Recruiter);
            sheet.Cell($"I{row}").SetValue(data.CreatedAt);
            sheet.Cell($"J{row}").SetValue(data.HasVehicle.YesOrNo());
        }
    }
}
