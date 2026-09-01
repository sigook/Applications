using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Company.Accounting;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Company)]
[ServiceFilter(typeof(CompanyIdFilter))]
public class InvoicesController(IInvoiceRepository invoiceRepository, ICompanyService companyService) : ControllerBase
{
    public const string RouteName = "api/company/accounting/Invoices";

    /// <summary>Gets the paginated list of invoices for the current company.</summary>
    /// <param name="filter">Filter and pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<InvoiceListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetCompanyInvoiceFilter filter) => Ok(await companyService.GetCompanyInvoices(filter));

    /// <summary>Gets the summary of a specific invoice by its identifier.</summary>
    /// <param name="id">Invoice identifier.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvoiceSummaryModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var invoice = await invoiceRepository.GetInvoiceSummaryById(id);
        if (invoice is null) return NotFound();
        return Ok(invoice);
    }
}
