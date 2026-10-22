using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Clients;

[ApiController]
[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class LogoController : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/clients/{profileId}/logo";

    /// <summary>Replaces the logo of a company profile with the uploaded image.</summary>
    /// <param name="companyService">Company service.</param>
    /// <param name="profileId">Identifier of the company profile.</param>
    [HttpPut]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromServices] ICompanyService companyService,
        Guid profileId)
    {
        var result = await companyService.UpdateLogo(profileId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
