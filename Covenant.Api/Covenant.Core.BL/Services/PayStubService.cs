using ClosedXML.Excel;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services
{
    public class PayStubService : IPayStubService
    {
        private readonly IPayStubRepository payStubRepository;

        public PayStubService(IPayStubRepository payStubRepository)
        {
            this.payStubRepository = payStubRepository;
        }

        public async Task<Result<ResultGenerateDocument<byte[]>>> GenerateT4(DateTime from, DateTime to)
        {
            var result = await payStubRepository.GetPayStubsByDates(from, to);
            using var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            GetDetail(result, workbook);
            GetReport(result, workbook);
            workbook.SaveAs(stream);
            return Result.Ok(new ResultGenerateDocument<byte[]>(stream.ToArray(), $"T4_Resume_{DateTime.Now.Year}.xlsx", string.Empty));
        }

        private void GetReport(IEnumerable<PayStubT4Model> result, XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("Report");
            var headerNames = new string[]
            {
                "No.",
                "Name",
                "Address",
                "SIN #",
                "Total Earnings",
                "CPP Employer",
                "EI Employeer",
                "Other Deductions",
                "CPP Employee",
                "EI Employee",
                "Fed Tax",
                "Prov Tax",
                "Total Tax",
                "Net Pay",
                "Total"
            };
            sheet.SetupHeaders(headerNames);
            var startAt = 2;
            for (int i = 0; i < result.Count(); i++)
            {
                var data = result.ElementAt(i);
                var totalTax = data.Items.Sum(i => i.Employee.FederalTax + i.Employee.ProvincialTax);
                var netPay = data.Items.Sum(i => i.TotalEarnings - i.Employee.Cpp - i.Employee.EI - i.Employee.FederalTax - i.Employee.ProvincialTax);
                var total = data.Items.Sum(i => i.Employer.Cpp + i.Employer.EI + i.Employee.Cpp + i.Employee.EI + i.Employee.FederalTax + i.Employee.ProvincialTax);
                sheet.Cell($"A{startAt}").SetValue(i + 1);
                sheet.Cell($"B{startAt}").SetValue(data.WorkerFullName.Trim());
                sheet.Cell($"C{startAt}").SetValue(data.Location);
                sheet.Cell($"D{startAt}").SetValue(data.Sin);
                sheet.Cell($"E{startAt}").SetValue(data.Items.Sum(i => i.TotalEarnings)).SetMoneyType();
                sheet.Cell($"F{startAt}").SetValue(data.Items.Sum(i => i.Employer.Cpp)).SetMoneyType();
                sheet.Cell($"G{startAt}").SetValue(data.Items.Sum(i => i.Employer.EI)).SetMoneyType();
                sheet.Cell($"H{startAt}").SetValue(data.Items.Sum(i => i.Employer.OtherDeductions)).SetMoneyType();
                sheet.Cell($"I{startAt}").SetValue(data.Items.Sum(i => i.Employee.Cpp)).SetMoneyType();
                sheet.Cell($"J{startAt}").SetValue(data.Items.Sum(i => i.Employee.EI)).SetMoneyType();
                sheet.Cell($"K{startAt}").SetValue(data.Items.Sum(i => i.Employee.FederalTax)).SetMoneyType();
                sheet.Cell($"L{startAt}").SetValue(data.Items.Sum(i => i.Employee.ProvincialTax)).SetMoneyType();
                sheet.Cell($"M{startAt}").SetValue(totalTax).SetMoneyType();
                sheet.Cell($"N{startAt}").SetValue(netPay).SetMoneyType();
                sheet.Cell($"O{startAt}").SetValue(total).SetMoneyType();
                startAt++;
            }
        }

        private void GetDetail(IEnumerable<PayStubT4Model> result, XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("Detail");
            var startAt = 1;
            for (int i = 0; i < result.Count(); i++)
            {
                var data = result.ElementAt(i);
                GenerateDetailTemplate(sheet, data, i, ref startAt);
            }
            startAt++;
            sheet.Cell($"P{startAt}").SetValue(result.Sum(r => r.Items.Sum(i => i.TotalEarnings))).SetMoneyType();
        }

        private void GenerateDetailTemplate(IXLWorksheet sheet, PayStubT4Model data, int index, ref int row)
        {
            Action<IXLCell> contentLabel = cell =>
            {
                cell.Style.Font.SetFontName("Arial Black");
                cell.Style.Font.SetFontSize(11);
                cell.Style.Font.SetBold(true);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            };
            Action<IXLCell> contentValue = cell =>
            {
                cell.Style.Font.SetFontName("Arial");
                cell.Style.Font.SetFontSize(11);
                cell.Style.Font.SetBold(true);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            };
            Action<IXLCell> contentHeader = cell =>
            {
                cell.Style.Font.SetFontName("Arial");
                cell.Style.Font.SetFontSize(11);
                cell.Style.Font.SetBold(true);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            };
            Action<IXLCell> tableHeader = cell =>
            {
                cell.WorksheetColumn().Width = 15;
                cell.Style.Font.SetFontName("Arial");
                cell.Style.Font.SetFontSize(11);
                cell.Style.Font.SetBold(true);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(166, 166, 166);
                cell.Style.Font.FontColor = XLColor.White;
            };
            Action<IXLCell> tableContent = cell =>
            {
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thick;
            };
            Action<IXLCell> emptyRow = cell =>
            {
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            };
            Action<IXLCell> tableFooter = cell =>
            {
                cell.Style.Font.SetBold(true);
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Double;
            };
            var items = data.Items.OrderBy(i => i.DatePaid);
            sheet.Cell($"A{row}").SetValue(index + 1).Config(cell =>
            {
                cell.Style.Font.SetFontName("Arial Black");
                cell.Style.Font.SetFontSize(11);
                cell.Style.Font.SetBold(true);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
                cell.Style.Font.FontColor = XLColor.Black;
            });
            sheet.Cell($"B{row}").SetValue("Name:").Config(contentLabel);
            sheet.Cell($"C{row}").SetValue(data.WorkerFullName.Trim()).Config(contentValue);
            sheet.Range($"C{row}:G{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell($"H{row}").SetValue("SIN #:").Config(contentLabel);
            sheet.Cell($"I{row}").SetValue(data.Sin).Config(contentValue);
            sheet.Range($"I{row}:L{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            row++;
            sheet.Cell($"B{row}").SetValue("Address:").Config(contentLabel);
            sheet.Cell($"C{row}").SetValue(data.Location).Config(contentValue);
            sheet.Range($"C{row}:L{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            row++;
            sheet.Cell($"D{row}").SetValue("Phone:").Config(contentLabel);
            sheet.Cell($"E{row}").SetValue(data.Phone).Config(contentValue);
            sheet.Range($"E{row}:G{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            row += 2;
            sheet.Cell($"F{row}").SetValue("EMPLOYER").Config(contentHeader);
            sheet.Range($"F{row}:H{row}").Row(1).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            sheet.Cell($"I{row}").SetValue("EMPLOYEE").Config(contentHeader);
            sheet.Range($"I{row}:L{row}").Row(1).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            row++;
            sheet.Cell($"B{row}").SetValue("No.").Config(tableHeader);
            sheet.Cell($"C{row}").SetValue("Paystub #").Config(tableHeader);
            sheet.Cell($"D{row}").SetValue("Date Paid").Config(tableHeader);
            sheet.Cell($"E{row}").SetValue("Total Earnings").Config(tableHeader);
            sheet.Cell($"F{row}").SetValue("CPP").Config(tableHeader);
            sheet.Cell($"G{row}").SetValue("EI").Config(tableHeader);
            sheet.Cell($"H{row}").SetValue("Others Deductions").Config(tableHeader);
            sheet.Cell($"I{row}").SetValue("CPP").Config(tableHeader);
            sheet.Cell($"J{row}").SetValue("EI").Config(tableHeader);
            sheet.Cell($"K{row}").SetValue("FED TAX").Config(tableHeader);
            sheet.Cell($"L{row}").SetValue("PROV TAX").Config(tableHeader);
            for (int i = 0; i < items.Count(); i++)
            {
                row++;
                var item = items.ElementAt(i);
                sheet.Cell($"B{row}").SetValue(i + 1).Config(tableContent);
                sheet.Cell($"C{row}").SetValue(item.PayStubNumber).Config(tableContent);
                sheet.Cell($"D{row}").SetValue(item.DatePaid.ToString("dd-MMM-yyyy"))
                    .Config(tableContent)
                    .Config(cell =>
                    {
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    });
                sheet.Cell($"E{row}").SetValue(item.TotalEarnings).SetMoneyType().Config(tableContent);
                sheet.Cell($"F{row}").SetValue(item.Employer.Cpp).SetMoneyType().Config(tableContent);
                sheet.Cell($"G{row}").SetValue(item.Employer.EI).SetMoneyType().Config(tableContent);
                sheet.Cell($"H{row}").SetValue(item.Employer.OtherDeductions).SetMoneyType().Config(tableContent);
                sheet.Cell($"I{row}").SetValue(item.Employee.Cpp).SetMoneyType().Config(tableContent);
                sheet.Cell($"J{row}").SetValue(item.Employee.EI).SetMoneyType().Config(tableContent);
                sheet.Cell($"K{row}").SetValue(item.Employee.FederalTax).SetMoneyType().Config(tableContent);
                sheet.Cell($"L{row}").SetValue(item.Employee.ProvincialTax).SetMoneyType().Config(tableContent);
            }
            row++;
            sheet.Cell($"B{row}").Config(emptyRow);
            sheet.Cell($"C{row}").Config(emptyRow);
            sheet.Cell($"D{row}").Config(emptyRow);
            sheet.Cell($"E{row}").Config(emptyRow);
            sheet.Cell($"F{row}").Config(emptyRow);
            sheet.Cell($"G{row}").Config(emptyRow);
            sheet.Cell($"H{row}").Config(emptyRow);
            sheet.Cell($"I{row}").Config(emptyRow);
            sheet.Cell($"J{row}").Config(emptyRow);
            sheet.Cell($"K{row}").Config(emptyRow);
            sheet.Cell($"L{row}").Config(emptyRow);
            row++;
            sheet.Cell($"E{row}").SetValue(items.Sum(i => i.TotalEarnings)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"F{row}").SetValue(items.Sum(i => i.Employer.Cpp)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"G{row}").SetValue(items.Sum(i => i.Employer.EI)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"H{row}").SetValue(items.Sum(i => i.Employer.OtherDeductions)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"I{row}").SetValue(items.Sum(i => i.Employee.Cpp)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"J{row}").SetValue(items.Sum(i => i.Employee.EI)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"K{row}").SetValue(items.Sum(i => i.Employee.FederalTax)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"L{row}").SetValue(items.Sum(i => i.Employee.ProvincialTax)).SetMoneyType().Config(tableFooter);
            sheet.Cell($"P{row}").SetValue(items.Sum(i => i.TotalEarnings)).SetMoneyType().Config(c => c.WorksheetColumn().Width = 15);
            row++;
            sheet.Cell($"L{row}").SetFormulaA1($"SUM(F{row - 1}:L{row - 1})").SetMoneyType().Config(tableFooter);
            row++;
        }
    }
}
