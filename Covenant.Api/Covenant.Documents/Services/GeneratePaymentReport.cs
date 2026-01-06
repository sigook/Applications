using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Utils.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public class GeneratePaymentReport : IRequest<ResultGenerateDocument<byte[]>>
{
    public GeneratePaymentReport(IReadOnlyList<WeeklyPayStubModel> model)
    {
        Model = model;
    }

    public IReadOnlyList<WeeklyPayStubModel> Model { get; set; }
}

public class GeneratePaymentReportHandler : IRequestHandler<GeneratePaymentReport, ResultGenerateDocument<byte[]>>
{
    private const string
        PayStubNumber = "A",
        FullName = "B",
        Description = "C",
        Quantity = "D",
        UnitPrice = "E",
        Total = "F",
        GrossPayment = "G",
        Vacations = "H",
        PublicHoliday = "I",
        TotalEarnings = "J",
        Cpp = "K",
        Ei = "L",
        FederalTax = "M",
        ProvincialTax = "N",
        OtherDeductions = "O",
        TotalDeductions = "P",
        TotalPaid = "Q",
        WeekEnding = "R",
        PaymentDate = "S",
        Companies = "T",
        Email = "U";

    private const int HeadRow = 2;

    private static readonly IEnumerable<string> _columns = new[]
    {
        PayStubNumber,
        FullName,
        Email,
        Description,
        Quantity,
        UnitPrice,
        Total,
        GrossPayment,
        Vacations,
        PublicHoliday,
        TotalEarnings,
        Cpp,
        Ei,
        FederalTax,
        ProvincialTax,
        OtherDeductions,
        TotalDeductions,
        TotalPaid,
        PaymentDate,
        Companies
    };

    private static readonly XLColor CustomGreen = XLColor.FromArgb(236, 240, 223);

    public async Task<ResultGenerateDocument<byte[]>> Handle(GeneratePaymentReport request, CancellationToken cancellationToken)
    {
        var list = request.Model;
        using var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");
        SetUpHeader(worksheet);
        SetUpWidth(worksheet);
        worksheet.Column(PaymentDate).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        var startIndex = HeadRow + 1;
        var color = XLColor.White;
        foreach (var payStub in list)
        {
            var itemsCount = payStub.Items.Count;
            for (var fIndex = 0; fIndex < itemsCount; fIndex++)
            {
                var itemModel = payStub.Items[fIndex];
                worksheet.Cell($"{Description}{startIndex}").SetValue(itemModel.Description);
                worksheet.Cell($"{Quantity}{startIndex}").SetValue(itemModel.Quantity);
                worksheet.Cell($"{UnitPrice}{startIndex}").SetValue(itemModel.UnitPrice).SetMoneyType();
                worksheet.Cell($"{Total}{startIndex}").SetValue(itemModel.Total).SetMoneyType();
                worksheet.Row(startIndex).Style.Fill.BackgroundColor = color;
                if (fIndex < itemsCount - 1) startIndex++;
            }

            worksheet.Cell($"{PayStubNumber}{startIndex}").SetValue(payStub.PayStubNumber);
            worksheet.Cell($"{FullName}{startIndex}").SetValue(payStub.FullName);
            worksheet.Cell($"{Email}{startIndex}").SetValue(payStub.Email);
            worksheet.Cell($"{GrossPayment}{startIndex}").SetValue(payStub.GrossPayment).SetMoneyType();
            worksheet.Cell($"{Vacations}{startIndex}").SetValue(payStub.Vacations).SetMoneyType();
            worksheet.Cell($"{PublicHoliday}{startIndex}").SetValue(payStub.PublicHoliday).SetMoneyType();
            worksheet.Cell($"{TotalEarnings}{startIndex}").SetValue(payStub.TotalEarnings).SetMoneyType();
            worksheet.Cell($"{Cpp}{startIndex}").SetValue(payStub.Cpp).SetMoneyType();
            worksheet.Cell($"{Ei}{startIndex}").SetValue(payStub.Ei).SetMoneyType();
            worksheet.Cell($"{FederalTax}{startIndex}").SetValue(payStub.FederalTax).SetMoneyType();
            worksheet.Cell($"{ProvincialTax}{startIndex}").SetValue(payStub.ProvincialTax).SetMoneyType();
            worksheet.Cell($"{OtherDeductions}{startIndex}").SetValue(payStub.OtherDeductions).SetMoneyType();
            if (payStub.OtherDeductionsDetail.Any())
            {
                worksheet.Cell($"{OtherDeductions}{startIndex}").CreateComment().AddText(string.Join("-", payStub.OtherDeductionsDetail));
            }
            worksheet.Cell($"{TotalDeductions}{startIndex}").SetValue(payStub.TotalDeductions).SetMoneyType();
            worksheet.Cell($"{TotalPaid}{startIndex}").SetValue(payStub.TotalPaid).SetMoneyType();
            worksheet.Cell($"{WeekEnding}{startIndex}").SetValue(payStub.WeedEnding.ToString("dd-MMM-yyyy"));
            worksheet.Cell($"{PaymentDate}{startIndex}").SetValue(payStub.PaymentDate.ToString("dd-MMM-yyyy"));
            worksheet.Cell($"{Companies}{startIndex}").SetValue(string.Join(",", payStub.Companies));
            worksheet.Row(startIndex).Style.Fill.BackgroundColor = color;
            startIndex++;
            color = color == XLColor.White ? CustomGreen : XLColor.White;
        }
        startIndex++;
        SetTotals(worksheet, startIndex);
        var valueA1 = $"Payment Date: {(list.FirstOrDefault()?.PaymentDate ?? default).ToLongDateString()}";
        SetValueA1(worksheet, valueA1);
        workbook.SaveAs(memoryStream);
        await Task.CompletedTask;
        return new ResultGenerateDocument<byte[]>(memoryStream.ToArray(), $"PaymentReport_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
    }

    private void SetUpHeader(IXLWorksheet worksheet)
    {
        worksheet.Row(HeadRow).Style.Font.Bold = true;
        worksheet.Row(HeadRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Row(HeadRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Cell($"{PayStubNumber}{HeadRow}").SetValue("PAY STUB #");
        worksheet.Cell($"{FullName}{HeadRow}").SetValue("FULL NAME");
        worksheet.Cell($"{Description}{HeadRow}").SetValue("DESCRIPTION");
        worksheet.Cell($"{Quantity}{HeadRow}").SetValue("QUANTITY");
        worksheet.Cell($"{UnitPrice}{HeadRow}").SetValue("UNIT PRICE");
        worksheet.Cell($"{Total}{HeadRow}").SetValue("TOTAL");
        worksheet.Cell($"{GrossPayment}{HeadRow}").SetValue("GROSS");
        worksheet.Cell($"{Vacations}{HeadRow}").SetValue("VACATIONS");
        worksheet.Cell($"{PublicHoliday}{HeadRow}").SetValue("PUBLIC HOLIDAY");
        worksheet.Cell($"{TotalEarnings}{HeadRow}").SetValue("TOTAL EARNINGS");
        worksheet.Cell($"{Cpp}{HeadRow}").SetValue("CPP");
        worksheet.Cell($"{Ei}{HeadRow}").SetValue("EI");
        worksheet.Cell($"{FederalTax}{HeadRow}").SetValue("FEDERAL TAX");
        worksheet.Cell($"{ProvincialTax}{HeadRow}").SetValue("PROVINCIAL TAX");
        worksheet.Cell($"{OtherDeductions}{HeadRow}").SetValue("OTHER DEDUCTIONS");
        worksheet.Cell($"{TotalDeductions}{HeadRow}").SetValue("TOTAL DEDUCTIONS");
        worksheet.Cell($"{TotalPaid}{HeadRow}").SetValue("TOTAL PAID");
        worksheet.Cell($"{WeekEnding}{HeadRow}").SetValue("WEEK ENDING");
        worksheet.Cell($"{PaymentDate}{HeadRow}").SetValue("PAYMENT DATE");
        worksheet.Cell($"{Companies}{HeadRow}").SetValue("COMPANY");
        worksheet.Cell($"{Email}{HeadRow}").SetValue("WORKER EMAIL");
    }

    private void SetUpWidth(IXLWorksheet worksheet)
    {
        worksheet.Column(PayStubNumber).Width = 9;
        worksheet.Column(FullName).Width = 25;
        worksheet.Column(Description).Width = 12;
        worksheet.Column(Quantity).Width = 9;
        worksheet.Column(UnitPrice).Width = 9;
        worksheet.Column(Total).Width = 9;
        worksheet.Column(GrossPayment).Width = 9;
        worksheet.Column(Vacations).Width = 9;
        worksheet.Column(PublicHoliday).Width = 12;
        worksheet.Column(TotalEarnings).Width = 13;
        worksheet.Column(Cpp).Width = 9;
        worksheet.Column(Ei).Width = 9;
        worksheet.Column(FederalTax).Width = 10;
        worksheet.Column(ProvincialTax).Width = 13;
        worksheet.Column(OtherDeductions).Width = 15;
        worksheet.Column(TotalDeductions).Width = 15;
        worksheet.Column(TotalPaid).Width = 9;
        worksheet.Column(PaymentDate).Width = 14;
        worksheet.Column(Companies).Width = 20;
        worksheet.Column(Email).Width = 29;
    }

    private void SetTotals(IXLWorksheet worksheet, int startIndex)
    {
        var values = new[]
        { 
            GrossPayment, 
            Vacations, 
            PublicHoliday, 
            TotalEarnings, 
            Cpp, 
            Ei, 
            FederalTax, 
            ProvincialTax, 
            OtherDeductions, 
            TotalDeductions, 
            TotalPaid 
        };

        foreach (var value in values)
        {
            worksheet.Cell($"{value}{startIndex}").SetFormulaA1($"=SUM({value}{2}:{value}{startIndex - 1})").SetMoneyType();
        }
    }

    private void SetValueA1(IXLWorksheet worksheet, string value)
    {
        IXLStyle xlStyle = worksheet.Range($"{_columns.First()}{HeadRow - 1}", $"{_columns.Last()}{HeadRow - 1}").Merge()
            .SetValue(value)
            .Style;
        xlStyle.Font.Bold = true;
        xlStyle.Font.SetFontSize(20);
    }
}
