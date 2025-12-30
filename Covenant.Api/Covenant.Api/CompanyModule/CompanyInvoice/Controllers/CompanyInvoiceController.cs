using Covenant.Api.Authorization;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyInvoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyInvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICompanyService companyService;

        public CompanyInvoiceController(IInvoiceRepository invoiceRepository, ICompanyService companyService)
        {
            _invoiceRepository = invoiceRepository;
            this.companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(GetCompanyInvoiceFilter filter) => Ok(await companyService.GetCompanyInvoices(filter));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var invoice = await _invoiceRepository.GetInvoiceSummaryById(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }
    }
}