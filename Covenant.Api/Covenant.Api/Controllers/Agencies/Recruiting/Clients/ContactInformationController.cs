using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Core.BL.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Clients;

[ApiController]
[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ContactInformationController : Controller
{
    public const string RouteName = "api/agency/recruiting/clients/{profileId}/contactinformation";

    /// <summary>Updates the contact information of a company profile.</summary>
    /// <param name="repository">Company repository.</param>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Updated contact information.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromServices] ICompanyService companyService,
        Guid profileId,
        [FromBody] CompanyProfileDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await companyService.UpdateContactInformation(profileId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
