using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    /// <summary>Gets the paginated wage history for a worker profile.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="pagination">Pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<PayStubHistoryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid workerProfileId, Pagination pagination)
    {
        return Ok(await _workerRepository.GetWageHistory(workerProfileId, pagination));
    }

    /// <summary>Gets the accumulated wage history for a worker profile up to a given row.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="rowNumber">Row number to accumulate up to.</param>
    [HttpGet("{rowNumber}")]
    [ProducesResponseType(typeof(PayStubHistoryAccumulated), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWageHistoryAccumulated([FromRoute] Guid workerProfileId, [FromRoute] int rowNumber)
    {
        var data = await _workerRepository.GetWageHistoryAccumulated(workerProfileId, rowNumber);
        return Ok(data);
    }
}