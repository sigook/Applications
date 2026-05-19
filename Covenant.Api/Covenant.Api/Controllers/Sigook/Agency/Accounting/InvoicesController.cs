using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/agency/accounting/[controller]")]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class InvoicesController : ControllerBase
{
    private readonly IAccountingService accountingService;

    public InvoicesController(IAccountingService accountingService)
    {
        this.accountingService = accountingService;
    }

    /// <summary>Gets the list of invoices matching the given filter with totals.</summary>
    /// <param name="filter">Filter criteria for invoices.</param>
    [HttpGet]
    [ProducesResponseType(typeof(InvoiceListModelWithTotals), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoices([FromQuery] GetInvoicesFilterV2 filter)
    {
        var data = await accountingService.GetInvoices(filter);
        return Ok(data);
    }

    /// <summary>Exports the filtered invoices to an Excel file.</summary>
    /// <param name="filter">Filter criteria for invoices.</param>
    [HttpGet("file")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoicesFile([FromQuery] GetInvoicesFilterV2 filter)
    {
        var file = await accountingService.GetInvoicesFile(filter);
        return File(file.Document, CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Generates a preview of an invoice without persisting it.</summary>
    /// <param name="model">Invoice data to preview.</param>
    [HttpPost("Preview")]
    [ProducesResponseType(typeof(InvoicePreviewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Preview([FromBody] CreateInvoiceModel model)
    {
        var result = await accountingService.PreviewInvoice(model);
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
        var result = await accountingService.CreateInvoice(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
