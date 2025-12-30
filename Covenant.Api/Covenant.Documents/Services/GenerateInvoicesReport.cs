using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Utils.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public class GenerateInvoicesReport : IRequest<ResultGenerateDocument<byte[]>>
{
    public GenerateInvoicesReport(IReadOnlyList<InvoiceListModel> model)
    {
        Model = model;
    }

    public IReadOnlyList<InvoiceListModel> Model { get; }

    public IEnumerable<string> Columns => new string[]
    {
        "N° Invoice",
        "Created At",
        "Company",
        "Total"
    };
}

public class GenerateInvoicesReportHandler : IRequestHandler<GenerateInvoicesReport, ResultGenerateDocument<byte[]>>
{
    public async Task<ResultGenerateDocument<byte[]>> Handle(GenerateInvoicesReport request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Report");
        sheet.SetupHeaders(request.Columns.ToArray());
        var startAt = 2;
        for (int i = 0; i < request.Model.Count; i++)
        {
            var row = startAt + i;
            var data = request.Model[i];
            SetValue(sheet, row, data);
        }
        workbook.SaveAs(memoryStream);
        await Task.CompletedTask;
        return new ResultGenerateDocument<byte[]>(memoryStream.ToArray(), $"Report_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
    }

    private void SetValue(IXLWorksheet sheet, int row, InvoiceListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.InvoiceNumber);
        sheet.Cell($"B{row}").SetValue(data.CreatedAt.ToString("yyyy-MM-dd"));
        sheet.Cell($"C{row}").SetValue(data.CompanyFullName);
        sheet.Cell($"D{row}").SetValue(data.TotalNet).SetMoneyType();
    }
}
