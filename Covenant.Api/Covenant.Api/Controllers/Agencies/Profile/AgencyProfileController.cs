using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Agencies;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Agencies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Profile;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ApiController]
[Produces("application/json")]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgencyProfileController(IAgencyRepository agencyRepository, IAgencyService agencyService) : ControllerBase
{
    public const string RouteName = "api/agency/profile";

    /// <summary>Gets the profile detail of the current agency.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAgencyProfile()
    {
        Guid agencyId = User.GetAgencyId();
        var agency = await agencyRepository.GetAgencyDetail(agencyId);
        if (agency is null) return NotFound();
        return Ok(agency);
    }

    /// <summary>Gets the agencies the caller has access to, for the agency filter of the grids.</summary>
    [HttpGet("agencies")]
    [ProducesResponseType(typeof(IEnumerable<AgencyOptionModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgencies() =>
        Ok(await agencyRepository.GetAgencyOptions(User.GetAgencyIds()));

    /// <summary>Updates the profile of the current agency.</summary>
    /// <param name="model">Updated agency data.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromBody] AgencyModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.UpdateAgencyProfile(User.GetAgencyId(), model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
