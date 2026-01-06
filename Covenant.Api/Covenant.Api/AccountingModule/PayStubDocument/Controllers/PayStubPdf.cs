using Covenant.Api.Shared.PayrollDocument.Models;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Pdf;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.HtmlTemplates.Views.Billing.Payroll;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Covenant.Api.AccountingModule.PayStubDocument.Controllers
{
    public class PayStubPdf
    {
        public delegate Task<IActionResult> OnPayStubPdf(string pdfPath, string fileName);
        public delegate Task<IActionResult> OnPayStubPdfAndModel(string pdfPath, string fileName, PayrollEmailViewModel model);
        private const string PdfContentType = MediaTypeNames.Application.Pdf;
        private const string PayStubTemplatePath = "/Views/Billing/Payroll/Payroll.cshtml";
        private readonly IPayStubsContainer _container;
        private readonly IPayStubRepository _repository;
        public IRazorViewToStringRenderer Renderer { get; }
        private readonly IPdfGeneratorService _generator;

        public PayStubPdf(
            IPayStubsContainer container,
            IPayStubRepository repository,
            IRazorViewToStringRenderer renderer,
            IPdfGeneratorService generator)
        {
            _container = container;
            _repository = repository;
            Renderer = renderer;
            _generator = generator;
        }

        public async Task<IActionResult> GetPdf(Guid payStubId, OnPayStubPdf func)
        {
            string fileName = payStubId.ToPayStubBlobName();
            string pdfPath = await _container.Download(fileName);
            if (!string.IsNullOrEmpty(pdfPath)) return await func(pdfPath, fileName);

            var model = await _repository.GetPayStubDetail(payStubId);
            if (model is null) return new NotFoundResult();
            pdfPath = await UploadPdf(model);
            return string.IsNullOrEmpty(pdfPath) ? new BadRequestResult() : await func(pdfPath, fileName);
        }

        public async Task<IActionResult> GetPdf(Guid payStubId, OnPayStubPdfAndModel func)
        {
            string fileName = payStubId.ToPayStubBlobName();
            string pdfPath = await _container.Download(fileName);

            var model = await _repository.GetPayStubDetail(payStubId);
            if (model is null) return new NotFoundResult();

            if (string.IsNullOrEmpty(pdfPath)) pdfPath = await UploadPdf(model);

            return string.IsNullOrEmpty(pdfPath) ? new BadRequestResult() :
                await func(pdfPath, fileName, new PayrollEmailViewModel(model.WorkerFullName, model.TotalNet, model.EndDate,
                    model.PaymentDate, model.WorkerEmail, model.PayrollNumber));
        }

        private async Task<string> UploadPdf(PayStubDetailModel model)
        {
            string fileName = model.Id.ToPayStubBlobName();
            string html = await Renderer.RenderViewToStringAsync(PayStubTemplatePath, model.ToPayrollViewModel());
            var pdf = await _generator.GeneratePdfFromHtml(new PdfParams(fileName, html));
            if (!pdf) return string.Empty;
            try
            {
                await _container.Upload(pdf.Value.Path, PdfContentType, new Dictionary<string, string>
                {
                    {nameof(model.PayrollNumberId), model.PayrollNumberId.ToString()}
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return pdf.Value.Path;
        }
    }
}