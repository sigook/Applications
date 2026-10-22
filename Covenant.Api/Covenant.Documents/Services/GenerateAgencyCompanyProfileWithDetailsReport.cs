using ClosedXML.Excel;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models;
using Covenant.Common.Utils.Extensions;
using Covenant.Documents.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public class GenerateAgencyCompanyProfileWithDetailsReport(IReadOnlyList<CompanyProfileWithDetailsModel> model) : IRequest<ResultGenerateDocument<MemoryStream>>
{
    public IReadOnlyList<CompanyProfileWithDetailsModel> Model { get; } = model;
}

public class GenerateAgencyCompanyProfileWithDetailsReportHandler : IRequestHandler<GenerateAgencyCompanyProfileWithDetailsReport, ResultGenerateDocument<MemoryStream>>
{
    public Task<ResultGenerateDocument<MemoryStream>> Handle(GenerateAgencyCompanyProfileWithDetailsReport request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();

        BuildCompaniesSheet(workbook, request.Model);
        BuildLocationsSheet(workbook, request.Model);
        BuildUsersSheet(workbook, request.Model);
        BuildContactsSheet(workbook, request.Model);
        BuildJobPositionsSheet(workbook, request.Model);

        workbook.SaveAs(memoryStream);
        var result = new ResultGenerateDocument<MemoryStream>(
            memoryStream, $"Companies_Details_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
        return Task.FromResult(result);
    }

    private static void BuildCompaniesSheet(XLWorkbook workbook, IReadOnlyList<CompanyProfileWithDetailsModel> companies)
    {
        var sheet = workbook.Worksheets.Add("Companies");
        sheet.SetupHeaders(["Company Name", "Industry", "Company Status", "Phone", "Email", "Website", "Location", "Created By", "Created At", "Updated By", "Updated At"]);

        var row = 2;
        foreach (var company in companies)
        {
            var c = company.Company;
            sheet.Cell($"A{row}").SetValue(c.FullName);
            sheet.Cell($"B{row}").SetValue(c.Industry);
            sheet.Cell($"C{row}").SetValue(c.CompanyStatus.ToString());
            sheet.Cell($"D{row}").SetValue(c.Phone);
            sheet.Cell($"E{row}").SetValue(c.Email);
            sheet.Cell($"F{row}").SetValue(c.Website);
            sheet.Cell($"G{row}").SetValue(company.Locations.FirstOrDefault()?.FormattedAddress);
            sheet.Cell($"H{row}").SetValue(c.CreatedBy);
            sheet.Cell($"I{row}").SetValue(c.CreatedAt);
            sheet.Cell($"J{row}").SetValue(c.UpdatedBy);
            sheet.Cell($"K{row}").SetValue(c.UpdatedAt);
            row++;
        }
    }

    private static void BuildLocationsSheet(XLWorkbook workbook, IReadOnlyList<CompanyProfileWithDetailsModel> companies)
    {
        var sheet = workbook.Worksheets.Add("Locations");
        sheet.SetupHeaders(["Company", "Address", "City", "Province", "Country", "Postal Code", "Entrance", "Main Intersection", "Billing"]);

        var row = 2;
        foreach (var company in companies)
        {
            foreach (var location in company.Locations)
            {
                sheet.Cell($"A{row}").SetValue(company.Company.FullName);
                sheet.Cell($"B{row}").SetValue(location.Address);
                sheet.Cell($"C{row}").SetValue(location.City);
                sheet.Cell($"D{row}").SetValue(location.Province);
                sheet.Cell($"E{row}").SetValue(location.Country);
                sheet.Cell($"F{row}").SetValue(location.PostalCode);
                sheet.Cell($"G{row}").SetValue(location.Entrance);
                sheet.Cell($"H{row}").SetValue(location.MainIntersection);
                sheet.Cell($"I{row}").SetValue(location.IsBilling ? "Yes" : "No");
                row++;
            }
        }
    }

    private static void BuildUsersSheet(XLWorkbook workbook, IReadOnlyList<CompanyProfileWithDetailsModel> companies)
    {
        var sheet = workbook.Worksheets.Add("Users");
        sheet.SetupHeaders(["Company", "Name", "Last Name", "Email", "Position", "Mobile", "Created At"]);

        var row = 2;
        foreach (var company in companies)
        {
            foreach (var user in company.Users)
            {
                sheet.Cell($"A{row}").SetValue(company.Company.FullName);
                sheet.Cell($"B{row}").SetValue(user.Name);
                sheet.Cell($"C{row}").SetValue(user.Lastname);
                sheet.Cell($"D{row}").SetValue(user.Email);
                sheet.Cell($"E{row}").SetValue(user.Position);
                sheet.Cell($"F{row}").SetValue(user.MobileNumber);
                sheet.Cell($"G{row}").SetValue(user.CreatedAt);
                row++;
            }
        }
    }

    private static void BuildContactsSheet(XLWorkbook workbook, IReadOnlyList<CompanyProfileWithDetailsModel> companies)
    {
        var sheet = workbook.Worksheets.Add("Contacts");
        sheet.SetupHeaders(["Company", "Company Status", "Title", "First Name", "Middle Name", "Last Name", "Position", "Email", "Mobile", "Office Number", "Office Ext"]);

        var row = 2;
        foreach (var company in companies)
        {
            foreach (var contact in company.Contacts)
            {
                sheet.Cell($"A{row}").SetValue(company.Company.FullName);
                sheet.Cell($"B{row}").SetValue(company.Company.CompanyStatus.ToString());
                sheet.Cell($"C{row}").SetValue(contact.Title);
                sheet.Cell($"D{row}").SetValue(contact.FirstName);
                sheet.Cell($"E{row}").SetValue(contact.MiddleName);
                sheet.Cell($"F{row}").SetValue(contact.LastName);
                sheet.Cell($"G{row}").SetValue(contact.Position);
                sheet.Cell($"H{row}").SetValue(contact.Email);
                sheet.Cell($"I{row}").SetValue(contact.MobileNumber);
                sheet.Cell($"J{row}").SetValue(contact.OfficeNumber);
                sheet.Cell($"K{row}").SetValue(contact.OfficeNumberExt);
                row++;
            }
        }
    }

    private static void BuildJobPositionsSheet(XLWorkbook workbook, IReadOnlyList<CompanyProfileWithDetailsModel> companies)
    {
        var sheet = workbook.Worksheets.Add("Job Positions");
        sheet.SetupHeaders(["Company", "Position", "Agency Rate", "Worker Rate", "Worker Rate Min", "Worker Rate Max", "Description", "Created By", "Created At"]);

        var row = 2;
        foreach (var company in companies)
        {
            foreach (var jp in company.JobPositions)
            {
                sheet.Cell($"A{row}").SetValue(company.Company.FullName);
                sheet.Cell($"B{row}").SetValue(jp.JobPosition);
                sheet.Cell($"C{row}").SetValue(jp.Rate).SetMoneyType();
                sheet.Cell($"D{row}").SetValue(jp.WorkerRate).SetMoneyType();
                sheet.Cell($"E{row}").SetValue(jp.WorkerRateMin).SetMoneyType();
                sheet.Cell($"F{row}").SetValue(jp.WorkerRateMax).SetMoneyType();
                sheet.Cell($"G{row}").SetValue(jp.Description);
                sheet.Cell($"H{row}").SetValue(jp.CreatedBy);
                sheet.Cell($"I{row}").SetValue(jp.CreatedAt);
                row++;
            }
        }
    }
}
