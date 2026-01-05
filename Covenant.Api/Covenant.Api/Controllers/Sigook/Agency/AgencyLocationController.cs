using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgencyLocationController : ControllerBase
{
    public const string RouteName = "api/Agency/Location";

    [HttpGet]
    public async Task<IActionResult> Get([FromServices] IAgencyRepository repository) =>
        Ok(await repository.GetLocations(User.GetAgencyId()));

    [HttpPost]
    public async Task<IActionResult> Post([FromServices] IAgencyRepository repository, [FromBody] LocationModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        Result<Location> rLocation = Location.Create(model.City?.Id, model.Address, model.PostalCode);
        if (!rLocation) return BadRequest(ModelState.AddErrors(rLocation.Errors));
        var agency = await repository.GetAgency(User.GetAgencyId());
        if (agency is null) return BadRequest();
        await repository.Create(new AgencyLocation(rLocation.Value, agency.Id, model.IsBilling));
        await repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = rLocation.Value.Id }, new LocationDetailModel { Id = rLocation.Value.Id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromServices] IAgencyRepository repository, Guid id)
    {
        LocationDetailModel model = await repository.GetLocationDetail(User.GetAgencyId(), id);
        if (model is null) return NotFound();
        return Ok(model);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromServices] IAgencyRepository repository,
        Guid id, [FromBody] LocationModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        Result<Location> rLocation = Location.Create(model.City?.Id, model.Address, model.PostalCode);
        if (!rLocation) return BadRequest(ModelState.AddErrors(rLocation.Errors));
        var agency = await repository.GetAgency(User.GetAgencyId());
        if (agency is null) return BadRequest();
        Result result = agency.UpdateLocation(id, rLocation.Value, model.IsBilling);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        repository.Update(agency);
        await repository.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromServices] IAgencyRepository repository, Guid id)
    {
        var agency = await repository.GetAgency(User.GetAgencyId());
        if (agency is null) return BadRequest();
        agency.DeleteLocation(id);
        repository.Update(agency);
        await repository.SaveChangesAsync();
        return Ok();
    }
}