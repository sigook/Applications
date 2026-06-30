using Covenant.Api.Authorization;
using Covenant.Api.Validators.Location;
using Covenant.Common.Models;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[Route("api/Location")]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
public class LocationTaxController(ILocationService locationService) : ControllerBase
{
    private readonly ILocationService _locationService = locationService;

    /// <summary>Gets the tax configured for a location.</summary>
    /// <param name="locationId">Location identifier.</param>
    [HttpGet("{locationId:guid}/tax")]
    [ProducesResponseType(typeof(LocationTaxModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocationTax([FromRoute] Guid locationId)
    {
        var tax = await _locationService.GetLocationTax(locationId);
        return Ok(tax);
    }

    /// <summary>Creates or updates the tax for a location.</summary>
    /// <param name="locationId">Location identifier.</param>
    /// <param name="model">Location tax data.</param>
    [HttpPut("{locationId:guid}/tax")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpsertLocationTax([FromRoute] Guid locationId, [FromBody] LocationTaxModel model)
    {
        var validationResult = await new LocationTaxModelValidator().ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return BadRequest(ModelState);
        }

        var result = await _locationService.UpsertLocationTax(locationId, model);
        if (!result)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }
}
