using Covenant.Api.Authorization;
using Covenant.Api.Utils;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Worker.TimeSheets;
[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Worker)]
[LogBadRequestFilter]
public class TimeSheetsController : ControllerBase
{
    public const string RouteName = "api/worker/requests/{requestId}/timesheet";
    private readonly ITimesheetService timesheetService;
    private readonly ITimesheetRepository _timesheetRepository;

    public TimeSheetsController(ITimesheetService timesheetService, ITimesheetRepository timesheetRepository)
    {
        this.timesheetService = timesheetService;
        _timesheetRepository = timesheetRepository;
    }

    /// <summary>
    /// Lists the timesheets of a request for the authenticated worker.
    /// </summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TimeSheetListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid requestId, Pagination pagination) => Ok(await _timesheetRepository.GetTimeSheetsForWorker(User.GetUserId(), requestId, pagination));

    /// <summary>
    /// Registers a clock-in/clock-out timesheet entry for the authenticated worker.
    /// </summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="model">Worker location data of the punch.</param>
    [HttpPost]
    [ProducesResponseType(typeof(RegisterTimeSheetResultModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromBody] WorkerLocationModel model)
    {
        var result = await timesheetService.Register(requestId, model);
        if (result) return Ok(result.Value);
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>
    /// Gets the next expected clock type (clock-in or clock-out) for a request.
    /// </summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="latitude">Current latitude of the worker.</param>
    /// <param name="longitude">Current longitude of the worker.</param>
    /// <param name="date">Optional date to evaluate the clock type for.</param>
    [HttpGet("clock-type/{latitude:double}/{longitude:double}")]
    [ProducesResponseType(typeof(ClockType), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClockType([FromRoute] Guid requestId, [FromRoute] double latitude, [FromRoute] double longitude, [FromQuery] DateTime? date)
    {
        var result = await timesheetService.GetClockType(requestId, latitude, longitude, date);
        return Ok(result.Value);
    }
}