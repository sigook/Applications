using ClosedXML.Excel;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models;
using Covenant.Common.Utils.Extensions;
using Covenant.Documents.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public class GeneratePayStubsReport(IReadOnlyList<PayStubListModel> model) : IRequest<ResultGenerateDocument<byte[]>>
{
    public IReadOnlyList<PayStubListModel> Model { get; } = model;

    public IEnumerable<string> Columns =>
    [
        "N° Pay Stub",
        "Created At",
        "Employee Full Name",
        "Number ID",
        "Total Paid"
    ];
}

public class GeneratePayStubsReportHandler : IRequestHandler<GeneratePayStubsReport, ResultGenerateDocument<byte[]>>
{
    public async Task<ResultGenerateDocument<byte[]>> Handle(GeneratePayStubsReport request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Report");
        sheet.SetupHeaders([.. request.Columns]);
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
        sheet.Cell($"D{row}").SetValue(data.NumberId);
        sheet.Cell($"E{row}").SetValue(data.TotalPaid).SetMoneyType();
    }
}
