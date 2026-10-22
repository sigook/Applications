using Covenant.Api.Authorization;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Repositories.Companies;
using Covenant.Core.BL.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Clients;

[ApiController]
[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class InvoiceNotesController(ICompanyRepository companyRepository, ICompanyService companyService) : Controller
{
    public const string RouteName = "api/agency/recruiting/clients/{profileId}/invoicenotes";

    /// <summary>Gets the invoice notes of a company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    [HttpGet]
    [ProducesResponseType(typeof(CompanyProfileInvoiceNotesModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid profileId)
    {
        string notes = await companyRepository.GetCompanyProfileInvoiceNotes(profileId);
        return Ok(new CompanyProfileInvoiceNotesModel { HtmlNotes = notes });
    }

    /// <summary>Updates the invoice notes of a company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Updated invoice notes.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid profileId, [FromBody] CompanyProfileInvoiceNotesModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await companyService.UpdateInvoiceNotes(profileId, model?.HtmlNotes);
        return Ok();
    }
}
