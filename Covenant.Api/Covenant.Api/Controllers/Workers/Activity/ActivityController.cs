using Covenant.Api.Authorization;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Models.Workers;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Workers.Activity;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Worker)]
public class ActivityController(IWorkerActivityService activityService) : ControllerBase
{
    public const string RouteName = "api/worker/activity";

    /// <summary>Gets the My Work cards of the authenticated worker: last pay stub, hours of the week and current assignment.</summary>
    /// <param name="weekStart">Any day of the week to summarize, in the worker's local time; the week runs Sunday to Saturday.</param>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(WorkerActivitySummaryModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime weekStart) =>
        Ok(await activityService.GetSummary(User.GetUserId(), weekStart));

    /// <summary>Lists the timesheets of the authenticated worker for one week across every assignment, with the regular, overtime and holiday split per request.</summary>
    /// <param name="weekStart">Any day of the week to list, in the worker's local time; the week runs Sunday to Saturday.</param>
    [HttpGet("timesheets")]
    [ProducesResponseType(typeof(WorkerWeekTimeSheetsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeSheets([FromQuery] DateTime weekStart) =>
        Ok(await activityService.GetWeekTimeSheets(User.GetUserId(), weekStart));
}
