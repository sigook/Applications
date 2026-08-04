using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Personnel;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class PersonnelController(IAgencyService agencyService, IAgencyRepository agencyRepository) : ControllerBase
{
    public const string RouteName = "api/agency/personnel";

    /// <summary>Gets all personnel of the current agency.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AgencyPersonnelModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        return Ok(await agencyRepository.GetAllPersonnel(User.GetAgencyId()));
    }

    /// <summary>Gets the roles that the current user is allowed to assign to a personnel of the current agency.</summary>
    [HttpGet("Roles")]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetAssignableRoles() => Ok(agencyService.GetAssignableRoles());

    /// <summary>Creates a new personnel record for the current agency.</summary>
    /// <param name="model">Personnel data.</param>
    [HttpPost]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] AgencyPersonnelModel model)
    {
        var result = await agencyService.CreateAgencyPersonnel(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Gets a personnel record of the current agency by its identifier.</summary>
    /// <param name="id">Identifier of the personnel record.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgencyPersonnelModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var model = await agencyRepository.GetPersonnel(User.GetAgencyId(), id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    /// <summary>Deletes a personnel record of the current agency.</summary>
    /// <param name="service">Identity server service used to remove the associated user or claim.</param>
    /// <param name="id">Identifier of the personnel record to delete.</param>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromServices] IIdentityServerService service,
        [FromRoute] Guid id)
    {
        Guid agencyId = User.GetAgencyId();
        var entity = await agencyRepository.GetPersonnel(id);
        if (entity is null || entity.AgencyId != agencyId) return NotFound();
        if (await agencyRepository.PersonnelHasRequests(entity.Id))
            return BadRequest(ModelState.AddError("This user cannot be deleted because they are still assigned as a recruiter on one or more orders. Please reassign those orders first."));
        if (await agencyRepository.PersonnelHasCompanies(entity.Id))
            return BadRequest(ModelState.AddError("This user cannot be deleted because they are still set as the sales representative for one or more clients. Please reassign those clients first."));
        await agencyRepository.DeletePersonnel(entity);
        await agencyRepository.SaveChangesAsync();
        var result = await service.DeleteUserOrClaim(entity.UserId, new IdModel(agencyId));
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
