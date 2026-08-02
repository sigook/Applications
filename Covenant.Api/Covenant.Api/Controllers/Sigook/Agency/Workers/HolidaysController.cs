using Covenant.Api.Authorization;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Workers;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class HolidaysController(IWorkerRepository workerRepository, IAgencyService agencyService) : Controller
{
    public const string RouteName = "api/agency/workers/{workerProfileId}/Holidays";

    /// <summary>Gets the holidays of the specified worker profile.</summary>
    /// <param name="workerProfileId">Identifier of the worker profile.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<WorkerProfileHolidayModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid workerProfileId)
    {
        return Ok(await workerRepository.GetWorkerProfileHoliday(workerProfileId));
    }

    /// <summary>Adds or updates a holiday for the specified worker profile.</summary>
    /// <param name="workerProfileId">Identifier of the worker profile.</param>
    /// <param name="model">Holiday data.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid workerProfileId, [FromBody] WorkerProfileHolidayModel model)
    {
        var result = await agencyService.AddUpdateWorkerHoliday(workerProfileId, model);
        if (result)
        {
            return Ok();
        }
        return BadRequest(result);
    }

    /// <summary>Creates a new holiday record for the country of the specified worker profile.</summary>
    /// <param name="workerProfileId">Identifier of the worker profile.</param>
    /// <param name="model">Holiday data.</param>
    [HttpPost("new-holiday")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateHoliday([FromRoute] Guid workerProfileId, [FromBody] WorkerProfileHolidayModel model)
    {
        model.WorkerProfileId = workerProfileId;
        var result = await agencyService.CreateHoliday(model);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }
}
