using ClosedXML.Excel;
using Covenant.Common.Models.Company;

namespace Covenant.Documents.Services
{
    public class GenerateAgencyCompanyProfileReport : GenerateAgencyReport<CompanyProfileListModel>
    {
        public GenerateAgencyCompanyProfileReport(IReadOnlyList<CompanyProfileListModel> model)
            : base(model)
        {
        }

        public override IEnumerable<string> Columns => new string[]
        {
            "Business Name",
            "Industry",
            "Company Status",
            "Contact Name",
            "Contact Role",
            "Phone",
            "Email",
            "Website",
            "Created By",
            "Created At",
            "Updated By",
            "Updated At"
        };
    }

    public class GenerateAgencyCompanyProfileReportHandler : GenerateAgencyReportHandler<GenerateAgencyCompanyProfileReport, CompanyProfileListModel>
    {
        public override void SetValue(IXLWorksheet sheet, int row, CompanyProfileListModel data)
        {
            sheet.Cell($"A{row}").SetValue(data.BusinessName);
            sheet.Cell($"B{row}").SetValue(data.Industry);
            sheet.Cell($"C{row}").SetValue(data.CompanyStatus.ToString());
            sheet.Cell($"D{row}").SetValue(data.ContactName);
            sheet.Cell($"E{row}").SetValue(data.ContactRole);
            sheet.Cell($"F{row}").SetValue(data.Phone);
            sheet.Cell($"G{row}").SetValue(data.Email);
            sheet.Cell($"H{row}").SetValue(data.Website);
            sheet.Cell($"I{row}").SetValue(data.CreatedBy);
            sheet.Cell($"J{row}").SetValue(data.CreatedAt);
            sheet.Cell($"K{row}").SetValue(data.UpdatedBy);
            sheet.Cell($"L{row}").SetValue(data.UpdatedAt);
        }
    }
}
