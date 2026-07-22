using ClosedXML.Excel;
using Covenant.Common.Models.Company;

namespace Covenant.Documents.Services;

public class GenerateAgencyCompanyProfileReport(IReadOnlyList<CompanyProfileListModel> model) : GenerateAgencyReport<CompanyProfileListModel>(model)
{
    public override IEnumerable<string> Columns =>
    [
        "Company Name",
        "Industry",
        "Company Status",
        "Phone",
        "Email",
        "Website",
        "Created By",
        "Created At",
        "Updated By",
        "Updated At"
    ];
}

public class GenerateAgencyCompanyProfileReportHandler : GenerateAgencyReportHandler<GenerateAgencyCompanyProfileReport, CompanyProfileListModel>
{
    public override void SetValue(IXLWorksheet sheet, int row, CompanyProfileListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.FullName);
        sheet.Cell($"B{row}").SetValue(data.Industry);
        sheet.Cell($"C{row}").SetValue(data.CompanyStatus.ToString());
        sheet.Cell($"D{row}").SetValue(data.Phone);
        sheet.Cell($"E{row}").SetValue(data.Email);
        sheet.Cell($"F{row}").SetValue(data.Website);
        sheet.Cell($"G{row}").SetValue(data.CreatedBy);
        sheet.Cell($"H{row}").SetValue(data.CreatedAt);
        sheet.Cell($"I{row}").SetValue(data.UpdatedBy);
        sheet.Cell($"J{row}").SetValue(data.UpdatedAt);
    }
}
