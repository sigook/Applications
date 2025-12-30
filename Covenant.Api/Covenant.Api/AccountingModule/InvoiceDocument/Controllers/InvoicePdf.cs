using Covenant.Api.AccountingModule.InvoiceDocument.Models;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Pdf;
using Covenant.Common.Utils.Extensions;
using System.Net.Mime;

namespace Covenant.Api.AccountingModule.InvoiceDocument.Controllers;

public class InvoicePdf
{
    private readonly IRazorViewToStringRenderer renderer;
    private readonly IPdfGeneratorService _generator;
    private readonly IInvoicesContainer _container;
    private const string InvoiceHtmlTemplate = "/Views/Billing/Invoice/Invoice.cshtml";

    public InvoicePdf(
        IRazorViewToStringRenderer renderer,
        IPdfGeneratorService generator,
        IInvoicesContainer container)
    {
        this.renderer = renderer;
        _generator = generator;
        _container = container;
    }

    public async Task<string> GetPdf(Guid invoiceId, Func<Task<InvoiceSummaryModel>> getModel)
    {
        string fileName = invoiceId.ToInvoiceBlobName();
        string pdfPath = await _container.Download(fileName);
        if (!string.IsNullOrEmpty(pdfPath)) return pdfPath;
        return await UploadPdf(await getModel());
    }

    private async Task<string> UploadPdf(InvoiceSummaryModel model)
    {
        if (model is null) return string.Empty;
        string fileName = model.Id.ToInvoiceBlobName();
        string html = await renderer.RenderViewToStringAsync(InvoiceHtmlTemplate, model.ToInvoiceViewModel());
        var pdf = await _generator.GeneratePdfFromHtml(new PdfParams(fileName, html));
        if (!pdf) return string.Empty;
        try
        {
            await _container.Upload(pdf.Value.Path, MediaTypeNames.Application.Pdf, new Dictionary<string, string>
                {
                    {nameof(model.InvoiceNumber), model.InvoiceNumber.Trim()}
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return pdf.Value.Path;
    }
}