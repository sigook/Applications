using Covenant.Api.Authorization;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyWorkerProfileHoliday.Controllers;

[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
[Route("api/agency-worker-profile-holiday")]
public class AgencyWorkerProfileHoliday : Controller
{
    private readonly IWorkerRepository _workerRepository;
    private readonly IAgencyService agencyService;

    public AgencyWorkerProfileHoliday(IWorkerRepository workerRepository, IAgencyService agencyService)
    {
        _workerRepository = workerRepository;
        this.agencyService = agencyService;
    }

    [HttpGet("{workerProfileId}")]
    public async Task<IActionResult> Get([FromRoute] Guid workerProfileId)
    {
        return Ok(await _workerRepository.GetWorkerProfileHoliday(workerProfileId));
    }


    [HttpPost("{workerProfileId}")]
    public async Task<IActionResult> Post([FromRoute] Guid workerProfileId, [FromBody] WorkerProfileHolidayModel model)
    {
        var result = await agencyService.AddUpdateWorkerHoliday(workerProfileId, model);
        if (result)
        {
            return Ok();
        }
        return BadRequest(result);
    }

    [HttpPost("new-holiday")]
    public async Task<IActionResult> CreateHoliday([FromBody] WorkerProfileHolidayModel model)
    {
        var result = await agencyService.CreateHoliday(model);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }
}