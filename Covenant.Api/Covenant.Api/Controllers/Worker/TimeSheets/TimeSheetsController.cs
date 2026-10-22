using Covenant.Api.Authorization;
using Covenant.Api.Utils;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Models.Request.Worker;
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
    [ProducesResponseType(typeof(PaginatedList<WorkerTimeSheetListModel>), StatusCodes.Status200OK)]
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
    /// Gets the next expected punch (clock-in or clock-out) for a request: clock-out while the latest punch is open and under 14 hours, none once today's punch is closed, clock-in otherwise.
    /// </summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="latitude">Current latitude of the worker.</param>
    /// <param name="longitude">Current longitude of the worker.</param>
    [HttpGet("clock-type/{latitude:double}/{longitude:double}")]
    [ProducesResponseType(typeof(ClockType), StatusCodes.Status200OK)]
    [Obsolete("Superseded by punch-status, which also returns the punch stamps and the week hours. Kept for mobile builds released before the punch-status endpoint.")]
    public async Task<IActionResult> GetClockType([FromRoute] Guid requestId, [FromRoute] double latitude, [FromRoute] double longitude)
    {
        var result = await timesheetService.GetClockType(requestId, latitude, longitude);
        return Ok(result.Value);
    }

    /// <summary>
    /// Gets everything the punch card needs in one call: the next expected punch, the clock-in/clock-out of the open or today's punch, and the hours worked since Sunday in the job's time zone.
    /// </summary>
    /// <param name="requestId">Identifier of the request.</param>
    /// <param name="latitude">Current latitude of the worker.</param>
    /// <param name="longitude">Current longitude of the worker.</param>
    [HttpGet("punch-status/{latitude:double}/{longitude:double}")]
    [ProducesResponseType(typeof(PunchStatusModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPunchStatus([FromRoute] Guid requestId, [FromRoute] double latitude, [FromRoute] double longitude)
    {
        var result = await timesheetService.GetPunchStatus(requestId, latitude, longitude);
        return Ok(result.Value);
    }
}