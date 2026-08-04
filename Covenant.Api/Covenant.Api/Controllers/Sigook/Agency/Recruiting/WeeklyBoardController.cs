using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Request.WeeklyBoard;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Recruiting;

[Route("api/agency/recruiting/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Recruiting)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class WeeklyBoardController(IWeeklyBoardService weeklyBoardService) : ControllerBase
{
    /// <summary>Gets the weekly board grouped by recruiter for the requested date range.</summary>
    /// <param name="filter">Date range to display (from/to).</param>
    [HttpGet]
    [ProducesResponseType(typeof(WeeklyBoardModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] WeeklyBoardFilter filter) =>
        Ok(await weeklyBoardService.GetWeeklyBoard(filter));

    /// <summary>Gets the weekly board for the current recruiter, including the workers sent to each order.</summary>
    /// <param name="filter">Date range to display (from/to).</param>
    [HttpGet("mine")]
    [ProducesResponseType(typeof(RecruiterWeeklyBoardModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine([FromQuery] WeeklyBoardFilter filter) =>
        Ok(await weeklyBoardService.GetRecruiterWeeklyBoard(filter));

    /// <summary>Gets every runner sent to an order across all recruiters and work days.</summary>
    /// <param name="requestId">Identifier of the order.</param>
    [HttpGet("{requestId:guid}/runners")]
    [ProducesResponseType(typeof(IEnumerable<WeeklyBoardRunnerModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderRunners(Guid requestId) =>
        Ok(await weeklyBoardService.GetOrderRunners(requestId));

    /// <summary>Assigns one or more recruiters to an order for one or more work days.</summary>
    /// <param name="model">Assignment data: order, work days and recruiters.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Assign([FromBody] AssignRecruitersModel model)
    {
        var result = await weeklyBoardService.AssignRecruiters(model);
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
        var result = await weeklyBoardService.UnassignRecruiter(requestId, recruiterId, workDate);
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
        var result = await weeklyBoardService.MoveAssignment(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Adds a runner sent by the current recruiter to an order for a specific work day.</summary>
    /// <param name="model">Runner data: order, work day, worker profile and runner type.</param>
    [HttpPost("runner")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRunner([FromBody] AddRunnerModel model)
    {
        var result = await weeklyBoardService.AddRunner(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
