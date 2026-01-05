using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Utils.Extensions;
using MediatR;

namespace Covenant.Documents.Services;

public abstract class GenerateAgencyReport<T> : IRequest<ResultGenerateDocument<MemoryStream>>
{
    public GenerateAgencyReport(IReadOnlyList<T> model)
    {
        Model = model;
    }

    public IReadOnlyList<T> Model { get; }
    public abstract IEnumerable<string> Columns { get; }
}

public abstract class GenerateAgencyReportHandler<T, TModel> : IRequestHandler<T, ResultGenerateDocument<MemoryStream>>
    where T : GenerateAgencyReport<TModel>
{
    public async Task<ResultGenerateDocument<MemoryStream>> Handle(T request, CancellationToken cancellationToken)
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
        return new ResultGenerateDocument<MemoryStream>(memoryStream, $"Report_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
    }

    public abstract void SetValue(IXLWorksheet sheet, int row, TModel data);
}
