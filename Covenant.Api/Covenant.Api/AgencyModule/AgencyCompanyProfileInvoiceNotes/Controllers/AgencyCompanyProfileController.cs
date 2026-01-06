using Covenant.Api.AgencyModule.AgencyCompanyProfileInvoiceNotes.Models;
using Covenant.Api.Authorization;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileInvoiceNotes.Controllers
{
    [Route("api/CompanyProfile/{id}/InvoiceNotes")]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCompanyProfileInvoiceNotesController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        public AgencyCompanyProfileInvoiceNotesController(ICompanyRepository companyRepository) => _companyRepository = companyRepository;

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            string notes = await _companyRepository.GetCompanyProfileInvoiceNotes(id);
            return Ok(new CompanyProfileInvoiceNotesModel { HtmlNotes = notes });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid id, [FromBody] CompanyProfileInvoiceNotesModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _companyRepository.UpdateCompanyProfileInvoiceNotes(id, model?.HtmlNotes);
            return Ok();
        }
    }
}