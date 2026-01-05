using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.WorkerProfileWageHistory;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[Route(Route)]
public class WorkerProfileWageHistoryController : ControllerBase
{
    private readonly IWorkerRepository _workerRepository;
    private const string Route = "api/WorkerProfile/{workerProfileId}/WageHistory";

    public WorkerProfileWageHistoryController(IWorkerRepository workerRepository) => _workerRepository = workerRepository;

    [HttpGet]
    public async Task<IActionResult> Get(Guid workerProfileId, Pagination pagination)
    {
        return Ok(await _workerRepository.GetWageHistory(workerProfileId, pagination));
    }

    [HttpGet("{rowNumber}")]
    public async Task<IActionResult> GetWageHistoryAccumulated([FromRoute] Guid workerProfileId, [FromRoute] int rowNumber)
    {
        var data = await _workerRepository.GetWageHistoryAccumulated(workerProfileId, rowNumber);
        return Ok(data);
    }
}