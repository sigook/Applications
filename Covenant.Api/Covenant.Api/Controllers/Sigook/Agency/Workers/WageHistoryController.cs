using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Workers;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[Route(RouteName)]
public class WageHistoryController(IWorkerRepository workerRepository) : ControllerBase
{
    public const string RouteName = "api/agency/workers/{workerProfileId:guid}/WageHistory";

    /// <summary>Gets the paginated wage history for a worker profile.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="pagination">Pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<PayStubHistoryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid workerProfileId, Pagination pagination) =>
        Ok(await workerRepository.GetWageHistory(workerProfileId, pagination));

    /// <summary>Gets the accumulated wage history for a worker profile up to a given row.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="rowNumber">Row number to accumulate up to.</param>
    [HttpGet("{rowNumber}")]
    [ProducesResponseType(typeof(PayStubHistoryAccumulated), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccumulated([FromRoute] Guid workerProfileId, [FromRoute] int rowNumber) =>
        Ok(await workerRepository.GetWageHistoryAccumulated(workerProfileId, rowNumber));
}
