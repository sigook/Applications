using Covenant.Api.Authorization;
using Covenant.Common.Models.Agency;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Personnel;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgenciesController : ControllerBase
{
    public const string RouteName = "api/agency/personnel/Agencies";

    /// <summary>Gets the personnel of the agencies the current user belongs to.</summary>
    /// <param name="repository">Agency repository.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonnelAgencyModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] IAgencyRepository repository) =>
        Ok(await repository.GetPersonnelAgency(User.GetUserId()));

    /// <summary>Sets the specified personnel record as the current user's primary agency.</summary>
    /// <param name="id">Identifier of the personnel record to set as primary.</param>
    /// <param name="repository">Agency repository.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Put(Guid id, [FromServices] IAgencyRepository repository)
    {
        foreach (var personnel in await repository.GetPersonnelByUserId(User.GetUserId()))
        {
            if (personnel.IsPrimary)
            {
                personnel.SetToNotPrimary();
                repository.Update(personnel);
            }

            if (personnel.Id != id) continue;
            personnel.SetToPrimary();
            repository.Update(personnel);
        }

        await repository.SaveChangesAsync();
        return Ok();
    }
}
