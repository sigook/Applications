using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Utils.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public class GeneratePayStubsReport : IRequest<ResultGenerateDocument<byte[]>>
{
    public GeneratePayStubsReport(IReadOnlyList<PayStubListModel> model)
    {
        Model = model;
    }

    public IReadOnlyList<PayStubListModel> Model { get; }

    public IEnumerable<string> Columns => new string[]
    {
        "N° Pay Stub",
        "Created At",
        "Worker Full Name",
        "Total Paid"
    };
}

public class GeneratePayStubsReportHandler : IRequestHandler<GeneratePayStubsReport, ResultGenerateDocument<byte[]>>
{
    public async Task<ResultGenerateDocument<byte[]>> Handle(GeneratePayStubsReport request, CancellationToken cancellationToken)
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
        return new ResultGenerateDocument<byte[]>(memoryStream.ToArray(), $"PayStubsReport_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
    }

    private void SetValue(IXLWorksheet sheet, int row, PayStubListModel data)
    {
        sheet.Cell($"A{row}").SetValue(data.PayStubNumber);
        sheet.Cell($"B{row}").SetValue(data.CreatedAt.ToString("yyyy-MM-dd"));
        sheet.Cell($"C{row}").SetValue(data.WorkerFullName);
        sheet.Cell($"D{row}").SetValue(data.TotalPaid).SetMoneyType();
    }
}
