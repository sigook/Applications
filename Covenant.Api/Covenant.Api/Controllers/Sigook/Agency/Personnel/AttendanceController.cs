using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Agency;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Personnel;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AttendanceController(IAgencyService agencyService) : ControllerBase
{
    public const string RouteName = "api/agency/attendance";

    /// <summary>Gets the clock-in/clock-out state of the current user for today, in the user's own time zone.</summary>
    /// <param name="timeZone">IANA time zone of the user's device (e.g. America/Bogota).</param>
    [HttpGet("today")]
    [ProducesResponseType(typeof(UserAttendanceTodayModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetToday([FromQuery] string timeZone)
    {
        var result = await agencyService.GetUserAttendanceToday(timeZone);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Clocks the current user in, or out when today's attendance is still open, at the user's local time. Only one attendance per day is allowed.</summary>
    /// <param name="timeZone">IANA time zone of the user's device (e.g. America/Bogota).</param>
    [HttpPost]
    [ProducesResponseType(typeof(UserAttendanceTodayModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Toggle([FromQuery] string timeZone)
    {
        var result = await agencyService.ToggleUserAttendance(timeZone);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Gets today's attendance of every user of the current agency who has clocked in.</summary>
    /// <param name="timeZone">IANA time zone of the caller's device, used to resolve "today".</param>
    [HttpGet("today/users")]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [ProducesResponseType(typeof(List<UserAttendanceTodayModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTodayForUsers([FromQuery] string timeZone)
    {
        var result = await agencyService.GetUserAttendancesToday(timeZone);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Gets the attendance report of the current agency's users with worked, lunch, regular and overtime hours per day.</summary>
    /// <param name="filter">User and date range (inclusive, at most one year).</param>
    [HttpGet("report")]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [ProducesResponseType(typeof(UserAttendanceReportModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReport([FromQuery] GetUserAttendanceReportFilter filter)
    {
        var result = await agencyService.GetUserAttendanceReport(filter);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Downloads the attendance report as an Excel file.</summary>
    /// <param name="filter">User and date range (inclusive, at most one year).</param>
    [HttpGet("report/file")]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [Produces("application/octet-stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReportFile([FromQuery] GetUserAttendanceReportFilter filter)
    {
        var result = await agencyService.GetUserAttendanceReportFile(filter);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return File(result.Value.Document.ToArray(), CovenantConstants.ExcelMime, result.Value.DocumentName);
    }

    /// <summary>Corrects the clock-in and clock-out of an attendance of a user of the current agency.</summary>
    /// <param name="id">Identifier of the attendance.</param>
    /// <param name="model">New clock-in and clock-out times and the reason of the change.</param>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyConfiguration.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateUserAttendanceModel model)
    {
        var result = await agencyService.UpdateUserAttendance(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
