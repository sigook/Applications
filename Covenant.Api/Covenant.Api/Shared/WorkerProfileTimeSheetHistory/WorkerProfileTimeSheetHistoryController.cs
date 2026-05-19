using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.WorkerProfileTimeSheetHistory;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[Route(Route)]
public class WorkerProfileTimeSheetHistoryController : ControllerBase
{
    private readonly ITimeSheetRepository _timeSheetRepository;
    public const string Route = "api/WorkerProfile/{workerProfileId}/TimeSheetHistory";

    public WorkerProfileTimeSheetHistoryController(ITimeSheetRepository timeSheetRepository) => _timeSheetRepository = timeSheetRepository;

    /// <summary>Gets the paginated timesheet history for a worker profile.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="pagination">Pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TimeSheetHistoryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid workerProfileId, Pagination pagination) =>
        Ok(await _timeSheetRepository.GetTimeSheetHistory(workerProfileId, pagination));

    /// <summary>Gets the accumulated timesheet history for a worker profile up to a given row.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="rowNumber">Row number to accumulate up to.</param>
    [HttpGet("{rowNumber}")]
    [ProducesResponseType(typeof(TimesheetHistoryAccumulated), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeSheetHistoryAccumulated([FromRoute] Guid workerProfileId, [FromRoute] int rowNumber)
    {
        var data = await _timeSheetRepository.GetTimesheetHistoryAccumulated(workerProfileId, rowNumber);
        return Ok(data);
    }
}