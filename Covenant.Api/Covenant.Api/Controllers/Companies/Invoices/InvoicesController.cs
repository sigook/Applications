using Covenant.Api.Authorization;
using Covenant.Common.Models.Accounting.Invoices.Companies;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models;
using Covenant.Core.BL.Interfaces.Accounting.Invoices;
using Covenant.Core.BL.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Covenant.Api.Controllers.Companies.Invoices;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Company)]
[ServiceFilter(typeof(CompanyIdFilter))]
public class InvoicesController(ICompanyService companyService, IInvoiceService invoiceService) : ControllerBase
{
    public const string RouteName = "api/company/invoices";

    /// <summary>Gets the paginated list of invoices for the current company.</summary>
    /// <param name="filter">Filter and pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<InvoiceListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetCompanyInvoiceFilter filter) => Ok(await invoiceService.GetCompanyInvoices(filter));

    /// <summary>Gets the summary of a specific invoice belonging to the current company.</summary>
    /// <param name="id">Invoice identifier.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvoiceSummaryModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var invoice = await companyService.GetCompanyInvoice(id);
        if (invoice is null) return NotFound();
        return Ok(invoice);
    }

    /// <summary>Returns the invoice document as a PDF file.</summary>
    /// <param name="invoiceId">Invoice identifier.</param>
    [HttpGet("{invoiceId:guid}/pdf")]
    [Produces("application/octet-stream", "application/json")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoicePdf(Guid invoiceId)
    {
        var document = await invoiceService.GetInvoicePdf(invoiceId);
        if (document is null) return NotFound();
        return File(document.Content, MediaTypeNames.Application.Pdf, document.FileName);
    }
}
