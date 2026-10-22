using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Requests.Runners;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Requests;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies.Recruiting.Runners;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Recruiting)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class RunnersController(
    IRunnerService runnerService,
    IRunnerRepository runnerRepository,
    IMediator mediator) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/runners";

    /// <summary>Gets the runners of every request of the current agency, paginated by request so no request is split across pages.</summary>
    /// <param name="filter">Runner filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyRunnersPagedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetAgencyRunnersFilter filter)
    {
        if (filter.OnlyMine) filter.Recruiter = User.GetNickname();
        return Ok(await runnerService.GetAgencyRunners(filter));
    }

    /// <summary>Changes the status of several runners at once, skipping the ones the transition is not valid for.</summary>
    /// <param name="model">Runners to change, the target status and its optional comments and start date.</param>
    [HttpPut("Status")]
    [ProducesResponseType(typeof(ChangeRunnersStatusResultModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeRunnersStatusModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await runnerService.ChangeRunnersStatus(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Generates and downloads an Excel report with every runner that matches the filter.</summary>
    /// <param name="filter">Runner filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile(GetAgencyRunnersFilter filter)
    {
        if (filter.OnlyMine) filter.Recruiter = User.GetNickname();
        var data = runnerRepository.GetAllAgencyRunners(User.GetAgencyId(), filter).ToList();
        var file = await mediator.Send(new GenerateAgencyRunnersReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
