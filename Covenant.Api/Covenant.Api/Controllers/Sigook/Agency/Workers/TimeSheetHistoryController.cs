using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Workers;

[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[Route(RouteName)]
public class TimeSheetHistoryController(ITimesheetRepository timeSheetRepository) : ControllerBase
{
    public const string RouteName = "api/agency/workers/{workerProfileId:guid}/TimeSheetHistory";

    /// <summary>Gets the paginated timesheet history for a worker profile.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="pagination">Pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TimeSheetHistoryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid workerProfileId, Pagination pagination) =>
        Ok(await timeSheetRepository.GetTimeSheetHistory(workerProfileId, pagination));

    /// <summary>Gets the accumulated timesheet history for a worker profile up to a given row.</summary>
    /// <param name="workerProfileId">Worker profile identifier.</param>
    /// <param name="rowNumber">Row number to accumulate up to.</param>
    [HttpGet("{rowNumber}")]
    [ProducesResponseType(typeof(TimesheetHistoryAccumulated), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccumulated([FromRoute] Guid workerProfileId, [FromRoute] int rowNumber) =>
        Ok(await timeSheetRepository.GetTimesheetHistoryAccumulated(workerProfileId, rowNumber));
}
