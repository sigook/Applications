using Covenant.Api.Authorization;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>Creates a new invoice recipient for the specified company profile.</summary>
        /// <param name="companyProfileId">Identifier of the company profile.</param>
        /// <param name="model">Invoice recipient data.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid companyProfileId, [FromBody] CompanyProfileInvoiceRecipientModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Guid id = await _companyRepository.CreateInvoiceRecipient(companyProfileId, model);
            await _companyRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { companyProfileId, id }, new { Id = id });
        }

        /// <summary>Gets the invoice recipients of the specified company profile.</summary>
        /// <param name="companyProfileId">Identifier of the company profile.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<CompanyProfileInvoiceRecipientModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] Guid companyProfileId) => Ok(await _companyRepository.GetInvoiceRecipients(companyProfileId));

        /// <summary>Updates an invoice recipient.</summary>
        /// <param name="id">Identifier of the invoice recipient to update.</param>
        /// <param name="model">Updated invoice recipient data.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CompanyProfileInvoiceRecipientModel model)
        {
            await _companyRepository.UpdateInvoiceRecipient(id, model);
            await _companyRepository.SaveChangesAsync();
            return Ok();
        }

        /// <summary>Deletes an invoice recipient.</summary>
        /// <param name="id">Identifier of the invoice recipient to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var invoiceRecipient = await _companyRepository.GetInvoiceRecipient(id);
            _companyRepository.Delete(invoiceRecipient);
            await _companyRepository.SaveChangesAsync();
            return Ok();
        }
    }
}