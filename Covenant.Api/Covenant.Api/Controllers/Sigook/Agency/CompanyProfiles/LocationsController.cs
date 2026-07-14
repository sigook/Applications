using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ApiController]
public class LocationsController(ICompanyRepository repository, ICompanyService companyService) : ControllerBase
{
    public const string RouteName = "api/agency/companyprofiles/{profileId}/Locations";

    /// <summary>Creates a new location for the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="model">Location data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyProfileLocationDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid profileId, [FromBody] CompanyProfileLocationDetailModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await companyService.CreateCompanyLocation(model, profileId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return CreatedAtAction(nameof(GetById), new { profileId, id = result.Value }, new CompanyProfileLocationDetailModel { Id = result.Value });
    }

    /// <summary>Gets the locations of the specified company profile.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LocationDetailModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid profileId) =>
        Ok(await repository.GetCompanyLocations(c => c.CompanyProfileId == profileId));

    /// <summary>Gets the detail of a company profile location by its identifier.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the location.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompanyProfileLocationDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid profileId, [FromRoute] Guid id)
    {
        CompanyProfileLocationDetailModel model = await repository.GetLocationDetail(id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Updates a company profile location.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the location to update.</param>
    /// <param name="model">Updated location data.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid profileId, [FromRoute] Guid id, [FromBody] CompanyProfileLocationDetailModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await companyService.UpdateCompanyLocation(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Deletes a company profile location.</summary>
    /// <param name="profileId">Identifier of the company profile.</param>
    /// <param name="id">Identifier of the location to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid profileId, [FromRoute] Guid id)
    {
        var location = await repository.GetLocation(id);
        repository.Delete(location);
        await repository.SaveChangesAsync();
        return Ok();
    }
}
