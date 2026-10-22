using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Locations;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Agencies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Profile;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class LocationsController : ControllerBase
{
    public const string RouteName = "api/agency/profile/locations";

    /// <summary>Gets all locations for the current agency.</summary>
    /// <param name="repository">Agency repository service.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<LocationDetailModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] IAgencyRepository repository) =>
        Ok(await repository.GetLocations(User.GetAgencyId()));

    /// <summary>Creates a new location for the current agency.</summary>
    /// <param name="agencyService">Agency service.</param>
    /// <param name="model">Location data to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(LocationDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] IAgencyService agencyService, [FromBody] LocationModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.CreateAgencyLocation(User.GetAgencyId(), model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new LocationDetailModel { Id = result.Value });
    }

    /// <summary>Gets the detail of a specific agency location by id.</summary>
    /// <param name="repository">Agency repository service.</param>
    /// <param name="id">Location identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LocationDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromServices] IAgencyRepository repository, Guid id)
    {
        LocationDetailModel model = await repository.GetLocationDetail(User.GetAgencyId(), id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates an existing agency location.</summary>
    /// <param name="agencyService">Agency service.</param>
    /// <param name="id">Location identifier.</param>
    /// <param name="model">Updated location data.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromServices] IAgencyService agencyService,
        Guid id, [FromBody] LocationModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await agencyService.UpdateAgencyLocation(User.GetAgencyId(), id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Deletes an agency location by id.</summary>
    /// <param name="agencyService">Agency service.</param>
    /// <param name="id">Location identifier.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromServices] IAgencyService agencyService, Guid id)
    {
        var result = await agencyService.DeleteAgencyLocation(User.GetAgencyId(), id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}