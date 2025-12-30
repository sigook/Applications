 using ClosedXML.Excel;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using MediatR;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Documents.Services
{
    public class GenerateWorkersReport : IRequest<ResultGenerateDocument<MemoryStream>>
    {
        public GenerateWorkersReport(IReadOnlyList<AgencyWorkerRequestModel> model)
        {
            Model = model;
        }

        public IReadOnlyList<AgencyWorkerRequestModel> Model { get; }
    }

    public class GenerateWorkersReportHandler : IRequestHandler<GenerateWorkersReport, ResultGenerateDocument<MemoryStream>>
    {
        public async Task<ResultGenerateDocument<MemoryStream>> Handle(GenerateWorkersReport request, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Report");
            var headerNames = new string[]
            {
                "Id",
                "FullName",
                "Mobile Number",
                "Social Insurance",
                "Social Insurance Due Date",
                "Start Working Data",
                "Status"
            };
            sheet.SetupHeaders(headerNames);
            var startAt = 2;
            for (int i = 0; i < request.Model.Count; i++)
            {
                var row = startAt + i;
                var data = request.Model[i];
                sheet.Cell($"A{row}").SetValue(data.NumberId).AdjustToContents();
                sheet.Cell($"B{row}").SetValue(data.Name).AdjustToContents();
                sheet.Cell($"C{row}").SetValue(data.MobileNumber).AdjustToContents();
                sheet.Cell($"D{row}").SetValue(data.SocialInsurance).AdjustToContents();
                sheet.Cell($"E{row}").SetValue(data.DueDate).AdjustToContents();
                sheet.Cell($"F{row}").SetValue(data.StartWorking).AdjustToContents();
                sheet.Cell($"G{row}").SetValue(data.Status).AdjustToContents();
            }
            workbook.SaveAs(memoryStream);
            await Task.CompletedTask;
            return new ResultGenerateDocument<MemoryStream>(memoryStream, $"WorkersReport_{DateTime.Now.ToFileTimeUtc()}.xlsx", string.Empty);
        }
    }
}
