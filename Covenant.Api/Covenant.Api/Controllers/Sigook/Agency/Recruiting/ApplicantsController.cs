using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Recruiting;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Recruiting)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class ApplicantsController(
    IRequestApplicantService requestApplicantService,
    IRequestRepository requestRepository,
    IMediator mediator) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/applicants";

    /// <summary>Gets the applicants of every request of the current agency, paginated by request so no request is split across pages.</summary>
    /// <param name="filter">Applicant filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyApplicantsPagedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetAgencyApplicantsFilter filter)
    {
        if (filter.OnlyMine) filter.Recruiter = User.GetNickname();
        return Ok(await requestApplicantService.GetAgencyApplicants(filter));
    }

    /// <summary>Changes the status of several applicants at once, skipping the ones the transition is not valid for.</summary>
    /// <param name="model">Applicants to change and the target status.</param>
    [HttpPut("Status")]
    [ProducesResponseType(typeof(ChangeApplicantsStatusResultModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeApplicantsStatusModel model)
    {
        if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestApplicantService.ChangeApplicantsStatus(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    /// <summary>Generates and downloads an Excel report with every applicant that matches the filter.</summary>
    /// <param name="filter">Applicant filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile(GetAgencyApplicantsFilter filter)
    {
        if (filter.OnlyMine) filter.Recruiter = User.GetNickname();
        var data = requestRepository.GetAllAgencyApplicants(User.GetAgencyId(), filter).ToList();
        var file = await mediator.Send(new GenerateAgencyApplicantsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
