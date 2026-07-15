using Covenant.Api.Authorization;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[ServiceFilter(typeof(AgencyIdFilter))]
public class InvoiceRecipientsController(ICompanyRepository companyRepository) : ControllerBase
{
    public const string RouteName = "api/agency/companyprofiles/{profileId}/InvoiceRecipients";

    /// <summary>Creates a new invoice recipient for the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Invoice recipient data.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid profileId, [FromBody] CompanyProfileInvoiceRecipientModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        Guid id = await companyRepository.CreateInvoiceRecipient(profileId, model);
        await companyRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { profileId, id }, new { Id = id });
    }

    /// <summary>Gets the invoice recipients of the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<CompanyProfileInvoiceRecipientModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid profileId) => Ok(await companyRepository.GetInvoiceRecipients(profileId));

    /// <summary>Updates an invoice recipient.</summary>
    /// <param name="id">Identifier of the invoice recipient to update.</param>
    /// <param name="model">Updated invoice recipient data.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CompanyProfileInvoiceRecipientModel model)
    {
        await companyRepository.UpdateInvoiceRecipient(id, model);
        await companyRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Deletes an invoice recipient.</summary>
    /// <param name="id">Identifier of the invoice recipient to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var invoiceRecipient = await companyRepository.GetInvoiceRecipient(id);
        companyRepository.Delete(invoiceRecipient);
        await companyRepository.SaveChangesAsync();
        return Ok();
    }
}
