using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Requests;

[Route("api/agency/requests/{requestId}/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Recruiting)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class RunnersController(IRunnerService runnerService, IRunnerRepository runnerRepository, IRequestRepository requestRepository) : ControllerBase
{
    /// <summary>Searches workers and candidates that can be added as runners for the request, excluding those already runners.</summary>
    /// <param name="requestId">Identifier of the request (order).</param>
    /// <param name="searchTerm">Term used to filter by name, email or number id.</param>
    [HttpGet("Search")]
    [ProducesResponseType(typeof(List<ApplicantSearchResultModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromRoute] Guid requestId, [FromQuery] string searchTerm) =>
        Ok(await requestRepository.SearchRunnerProspects(User.GetAgencyId(), requestId, searchTerm));

    /// <summary>Gets a paginated list of runners for the specified request.</summary>
    /// <param name="requestId">Identifier of the request (order).</param>
    /// <param name="filter">Runner filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RunnerListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid requestId, [FromQuery] GetRunnersFilter filter)
    {
        filter.RequestId = requestId;
        return Ok(await runnerRepository.GetRunners(User.GetAgencyId(), filter));
    }

    /// <summary>Gets the detail of a runner, including its status history and interviews.</summary>
    /// <param name="id">Identifier of the runner.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RunnerDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var runner = await runnerRepository.GetRunnerDetail(id);
        if (runner is null) return NotFound();
        return Ok(runner);
    }

    /// <summary>Creates a runner (from a candidate or a worker) for the specified request.</summary>
    /// <param name="requestId">Identifier of the request (order).</param>
    /// <param name="model">Runner data identifying a candidate or a worker profile and its type.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromBody] RunnerCreateModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await runnerService.CreateRunner(requestId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Changes the status of a runner and appends an entry to its status history.</summary>
    /// <param name="id">Identifier of the runner.</param>
    /// <param name="model">New status and optional comments and starting date.</param>
    [HttpPut("{id}/Status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus([FromRoute] Guid id, [FromBody] ChangeRunnerStatusModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await runnerService.ChangeStatus(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Adds an interview to a runner. Only allowed when the runner status is 'Interview scheduled' or 'Interview rescheduled'.</summary>
    /// <param name="id">Identifier of the runner.</param>
    /// <param name="model">Interview data: scheduled date, type, interviewer and notes.</param>
    [HttpPost("{id}/Interview")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddInterview([FromRoute] Guid id, [FromBody] RunnerInterviewCreateModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await runnerService.AddInterview(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Reschedules an existing interview of a runner.</summary>
    /// <param name="id">Identifier of the runner.</param>
    /// <param name="interviewId">Identifier of the interview to reschedule.</param>
    /// <param name="model">New scheduled date.</param>
    [HttpPut("{id}/Interview/{interviewId}/Reschedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RescheduleInterview([FromRoute] Guid id, [FromRoute] Guid interviewId, [FromBody] RunnerInterviewRescheduleModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await runnerService.RescheduleInterview(id, interviewId, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
