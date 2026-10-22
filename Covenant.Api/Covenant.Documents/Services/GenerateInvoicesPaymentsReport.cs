using ClosedXML.Excel;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models;
using Covenant.Documents.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public class GenerateInvoicesPaymentsReport(IReadOnlyList<InvoiceListModel> model) : IRequest<ResultGenerateDocument<byte[]>>
{
    public IReadOnlyList<InvoiceListModel> Model { get; } = model;

    public IEnumerable<string> Columns =>
    [
        "Invoice Date",
        "Invoice",
        "Company",
        "B4 Tax",
        "Tax",
        "Amount",
        "Status",
        "Net",
        "Due Date",
        "Due Days",
        "Payment Date"
    ];
}

public class GenerateInvoicesPaymentsReportHandler : IRequestHandler<GenerateInvoicesPaymentsReport, ResultGenerateDocument<byte[]>>
{
    private const string DateFormat = "yyyy-MM-dd";

    public async Task<ResultGenerateDocument<byte[]>> Handle(GenerateInvoicesPaymentsReport request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Payments");
        sheet.SetupHeaders(request.Columns.ToArray());
        var startAt = 2;
        for (int i = 0; i < request.Model.Count; i++)
        {
            SetValue(sheet, startAt + i, request.Model[i]);
        }
        workbook.SaveAs(memoryStream);
        await Task.CompletedTask;
        return new ResultGenerateDocument<byte[]>(memoryStream.ToArray(), $"Payments_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
    }

    private static void SetValue(IXLWorksheet sheet, int row, InvoiceListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.CreatedAt.ToString(DateFormat));
        sheet.Cell($"B{row}").SetValue(data.InvoiceNumber);
        sheet.Cell($"C{row}").SetValue(data.CompanyFullName);
        sheet.Cell($"D{row}").SetValue(data.SubTotal).SetMoneyType();
        sheet.Cell($"E{row}").SetValue(data.Tax).SetMoneyType();
        sheet.Cell($"F{row}").SetValue(data.TotalNet).SetMoneyType();
        sheet.Cell($"G{row}").SetValue(data.Status.ToString());
        sheet.Cell($"H{row}").SetValue(data.PaymentTermDays);
        sheet.Cell($"I{row}").SetValue(data.DueDate.ToString(DateFormat));
        sheet.Cell($"J{row}").SetValue(data.DueDays);
        sheet.Cell($"K{row}").SetValue(data.PaidAt?.ToString(DateFormat) ?? string.Empty);
    }
}
