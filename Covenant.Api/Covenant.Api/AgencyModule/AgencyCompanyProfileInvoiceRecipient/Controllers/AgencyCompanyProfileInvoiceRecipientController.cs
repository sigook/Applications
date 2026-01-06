using Covenant.Api.Authorization;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyCompanyProfileInvoiceRecipient.Controllers
{
    [Route("api/CompanyProfile/{companyProfileId}/InvoiceRecipient")]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ApiController]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyCompanyProfileInvoiceRecipientController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        public AgencyCompanyProfileInvoiceRecipientController(ICompanyRepository companyRepository) => _companyRepository = companyRepository;

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] Guid companyProfileId, [FromBody] CompanyProfileInvoiceRecipientModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Guid id = await _companyRepository.CreateInvoiceRecipient(companyProfileId, model);
            await _companyRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { companyProfileId, id }, new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid companyProfileId) => Ok(await _companyRepository.GetInvoiceRecipients(companyProfileId));

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CompanyProfileInvoiceRecipientModel model)
        {
            await _companyRepository.UpdateInvoiceRecipient(id, model);
            await _companyRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var invoiceRecipient = await _companyRepository.GetInvoiceRecipient(id);
            _companyRepository.Delete(invoiceRecipient);
            await _companyRepository.SaveChangesAsync();
            return Ok();
        }
    }
}