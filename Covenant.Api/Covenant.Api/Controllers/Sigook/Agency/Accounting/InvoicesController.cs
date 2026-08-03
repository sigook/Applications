using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services.Accounting.Invoices;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/agency/accounting/[controller]")]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class InvoicesController(InvoiceServiceFactory invoiceServiceFactory, IPayStubRepository payStubRepository) : ControllerBase
{
    private readonly Lazy<Task<IInvoiceService>> invoiceService = new Lazy<Task<IInvoiceService>>(invoiceServiceFactory.Resolve);

    /// <summary>Gets the list of invoices matching the given filter with totals.</summary>
    /// <param name="filter">Filter criteria for invoices.</param>
    [HttpGet]
    [ProducesResponseType(typeof(InvoiceListModelWithTotals), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoices([FromQuery] GetInvoicesFilterV2 filter)
    {
        var service = await invoiceService.Value;
        var data = await service.GetInvoices(filter);
        return Ok(data);
    }

    /// <summary>Exports the filtered invoices to an Excel file.</summary>
    /// <param name="filter">Filter criteria for invoices.</param>
    [HttpGet("file")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoicesFile([FromQuery] GetInvoicesFilterV2 filter)
    {
        var service = await invoiceService.Value;
        var file = await service.GetInvoicesFile(filter);
        return File(file.Document, CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Generates a preview of an invoice without persisting it.</summary>
    /// <param name="model">Invoice data to preview.</param>
    [HttpPost("Preview")]
    [ProducesResponseType(typeof(InvoicePreviewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Preview([FromBody] CreateInvoiceModel model)
    {
        var service = await invoiceService.Value;
        var result = await service.PreviewInvoice(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Creates a new invoice.</summary>
    /// <param name="model">Invoice data to create.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateInvoiceModel model)
    {
        var service = await invoiceService.Value;
        var result = await service.CreateInvoice(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Generates and returns the invoice document as a PDF file.</summary>
    /// <param name="invoiceId">Identifier of the invoice.</param>
    [HttpGet("{invoiceId:guid}/pdf")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoicePdf(Guid invoiceId)
    {
        var service = await invoiceService.Value;
        var document = await service.GetInvoicePdf(invoiceId);
        if (document is null) return NotFound();
        return File(document.Content, MediaTypeNames.Application.Pdf, document.FileName);
    }

    /// <summary>Gets the pay stubs linked to an invoice, including delete-warning information.</summary>
    /// <param name="invoiceId">Identifier of the invoice.</param>
    [HttpGet("{invoiceId:guid}/paystubs")]
    [ProducesResponseType(typeof(List<PayStubDeleteWarningListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayStubs(Guid invoiceId) =>
        Ok(await payStubRepository.GetPayStubs(invoiceId));

    /// <summary>Generates the invoice PDF and emails it, with optional attachments, to the client.</summary>
    /// <param name="invoiceId">Identifier of the invoice.</param>
    /// <param name="model">Email content, recipients and attachment files.</param>
    [HttpPost("{invoiceId:guid}/email")]
    [Consumes("multipart/form-data")]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendInvoiceEmail(Guid invoiceId, [FromForm] InvoiceEmailModel model)
    {
        var service = await invoiceService.Value;
        var result = await service.SendInvoiceEmail(invoiceId, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddError(result.StringErrors));
    }

    /// <summary>Deletes an invoice and its related pay stubs, and notifies the accounting team.</summary>
    /// <param name="id">Identifier of the invoice to delete.</param>
    /// <param name="model">The pay stubs to delete alongside the invoice.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, [FromBody] DeleteInvoiceModel model)
    {
        var service = await invoiceService.Value;
        await service.DeleteInvoice(id, model);
        return Ok();
    }
}
