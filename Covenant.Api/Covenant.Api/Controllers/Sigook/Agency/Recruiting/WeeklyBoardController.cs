using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Request.WeeklyBoard;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Recruiting;

[Route("api/agency/recruiting/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class WeeklyBoardController(IWeeklyBoardService weeklyBoardService) : ControllerBase
{
    /// <summary>Gets the weekly board grouped by recruiter for the requested date range.</summary>
    /// <param name="filter">Date range to display (from/to).</param>
    [HttpGet]
    [ProducesResponseType(typeof(WeeklyBoardModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] WeeklyBoardFilter filter) =>
        Ok(await weeklyBoardService.GetWeeklyBoard(User.GetAgencyId(), filter));

    /// <summary>Gets the weekly board for the current recruiter, including the workers sent to each order.</summary>
    /// <param name="filter">Date range to display (from/to).</param>
    [HttpGet("mine")]
    [ProducesResponseType(typeof(RecruiterWeeklyBoardModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine([FromQuery] WeeklyBoardFilter filter) =>
        Ok(await weeklyBoardService.GetRecruiterWeeklyBoard(User.GetAgencyId(), filter));

    /// <summary>Assigns one or more recruiters to an order for one or more work days.</summary>
    /// <param name="model">Assignment data: order, work days and recruiters.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Assign([FromBody] AssignRecruitersModel model)
    {
        var result = await weeklyBoardService.AssignRecruiters(User.GetAgencyId(), model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Removes a recruiter assignment from an order for a specific work day.</summary>
    /// <param name="requestId">Identifier of the order.</param>
    /// <param name="recruiterId">Identifier of the recruiter to unassign.</param>
    /// <param name="workDate">Work day of the assignment to remove.</param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unassign([FromQuery] Guid requestId, [FromQuery] Guid recruiterId, [FromQuery] DateTime workDate)
    {
        var result = await weeklyBoardService.UnassignRecruiter(User.GetAgencyId(), requestId, recruiterId, workDate);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Moves a recruiter assignment to another recruiter and/or work day, keeping its dispatched workers.</summary>
    /// <param name="model">Move data: order, source recruiter/day and target recruiter/day.</param>
    [HttpPost("move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Move([FromBody] MoveAssignmentModel model)
    {
        var result = await weeklyBoardService.MoveAssignment(User.GetAgencyId(), model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Adds one or more workers sent by the current recruiter to an order for a specific work day.</summary>
    /// <param name="model">Dispatch data: order, work day and worker profiles.</param>
    [HttpPost("dispatch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddWorkers([FromBody] DispatchWorkersModel model)
    {
        var result = await weeklyBoardService.AddWorkers(User.GetAgencyId(), model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Removes a worker sent by the current recruiter from an order for a specific work day.</summary>
    /// <param name="requestId">Identifier of the order.</param>
    /// <param name="workDate">Work day of the dispatch.</param>
    /// <param name="workerProfileId">Identifier of the worker profile to remove.</param>
    [HttpDelete("dispatch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveWorker([FromQuery] Guid requestId, [FromQuery] DateTime workDate, [FromQuery] Guid workerProfileId)
    {
        var result = await weeklyBoardService.RemoveWorker(User.GetAgencyId(), requestId, workDate, workerProfileId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
