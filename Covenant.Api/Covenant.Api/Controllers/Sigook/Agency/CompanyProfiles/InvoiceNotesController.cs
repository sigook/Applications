using Covenant.Api.Authorization;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class InvoiceNotesController(ICompanyRepository companyRepository) : Controller
{
    public const string RouteName = "api/agency/companyprofiles/{profileId}/InvoiceNotes";

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
        await companyRepository.UpdateCompanyProfileInvoiceNotes(profileId, model?.HtmlNotes);
        return Ok();
    }
}
