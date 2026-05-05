using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequest.Controllers;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgencyRequestController : ControllerBase
{
    public const string RouteName = "api/AgencyRequest";

    private readonly IRequestService requestService;
    private readonly IAgencyService agencyService;
    private readonly IMediator mediator;

    public AgencyRequestController(IMediator mediator, IRequestService requestService, IAgencyService agencyService)
    {
        this.mediator = mediator;
        this.requestService = requestService;
        this.agencyService = agencyService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromServices] IRequestRepository repository, GetRequestForAgencyFilter pagination)
    {
        var agencyId = User.GetAgencyId();
        if (pagination.OnlyMine)
        {
            pagination.Recruiter = User.GetNickname();
        }
        if (pagination.AgencyId.HasValue)
        {
            agencyId = pagination.AgencyId.Value;
        }
        pagination.HasPermissionToSeeInternalOrders = User.IsPayrollManager();
        return Ok(await repository.GetRequestsForAgency(agencyId, pagination));
    }

    [HttpGet("all")]
    public IActionResult GetAll([FromServices] IRequestRepository repository, GetRequestForAgencyFilter pagination)
    {
        var data = repository.GetAllRequestsForAgency(User.GetAgencyId(), pagination).ToList();
        return Ok(data);
    }

    [HttpGet("File")]
    public async Task<IActionResult> GetFile([FromServices] IRequestRepository repository, GetRequestForAgencyFilter pagination)
    {
        if (pagination.OnlyMine)
        {
            pagination.Recruiter = User.GetNickname();
        }
        pagination.HasPermissionToSeeInternalOrders = User.IsPayrollManager();
        var data = repository.GetAllRequestsForAgency(User.GetAgencyId(), pagination).ToList();
        var file = await mediator.Send(new GenerateAgencyRequestsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromServices] IRequestRepository repository, Guid id) =>
        this.GetByIdResult(await repository.GetRequestDetailForAgency(id));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RequestCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.CreateRequest(model);
        if (result) return CreatedAtAction("GetById", "AgencyRequest", new { id = result.Value }, new AgencyRequestDetailModel { Id = result.Value });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] RequestCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.UpdateRequest(id, model);
        if (result) return CreatedAtAction("GetById", "AgencyRequest", new { id }, new AgencyRequestDetailModel { Id = id });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    [HttpPut("{id:guid}/IsAsap")]
    public async Task<IActionResult> IsAsap([FromRoute] Guid id)
    {
        var result = await requestService.UpdateIsAsap(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPut("is-asap")]
    public async Task<IActionResult> UpdateIsAsapRequests([FromBody] RequestsQuickUpdate requestsQuickUpdate)
    {
        var result = await requestService.UpdateIsAsapRequests(requestsQuickUpdate);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPut("{id}/IncreaseWorkersQuantityByOne")]
    public async Task<IActionResult> IncreaseWorkersQuantityByOne([FromRoute] Guid id)
    {
        var result = await agencyService.IncreaseWorkersQuantityByOne(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPut("{id}/ReduceWorkersQuantityByOne")]
    public async Task<IActionResult> ReduceWorkersQuantityByOne([FromRoute] Guid id)
    {
        var result = await requestService.ReduceWorkerQuantityByOne(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPut("{id}/PunchCardVisibilityStatusInApp")]
    public async Task<IActionResult> PunchCardVisibilityStatusInApp([FromRoute] Guid id)
    {
        var result = await requestService.PunchCardUpdateVisibilityStatusInApp(id);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPut("{id}/Cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] RequestCancellationDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.CancelRequest(id, model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPut("bulk-cancel")]
    public async Task<IActionResult> BulkCancel([FromBody] BulkRequestCancellation model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.BulkCancelRequests(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }

    [HttpPut("bulk-recruiters")]
    public async Task<IActionResult> BulkRecruiters([FromBody] BulkRequestRecruiters model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.BulkUpdateRecruiters(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPut("{id:guid}/Open")]
    public async Task<IActionResult> Open([FromRoute] Guid id)
    {
        var result = await requestService.OpenRequest(id, User.GetNickname());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    [HttpPost("{id:guid}/SendInvitation")]
    public async Task<IActionResult> SendInvitation([FromServices] IRequestRepository repository, [FromServices] ITimeService timeService, [FromRoute] Guid id)
    {
        var request = await repository.GetRequest(r => r.Id == id);
        if (request is null || !request.CanBeUpdated) return BadRequest(ModelState.AddError(ApiResources.RequestNotAvailable));

        Guid agencyId = User.GetAgencyId();
        if (request.AgencyId != agencyId) return Unauthorized();

        DateTime now = timeService.GetCurrentDateTime();
        Result canBeSendIt = request.CanInvitationBeSendIt(now);
        if (!canBeSendIt) return BadRequest(ModelState.AddErrors(canBeSendIt.Errors));

        await requestService.SendInvitationToApply(new InvitationToApplyModel
        {
            RequestId = request.Id,
            JobTitle = request.JobTitle,
            Description = request.Description,
            Requirements = request.Requirements,
            City = request.JobLocation?.City?.Value,
            Rate = request.WorkerRate.HasValue ? request.WorkerRate.Value.ToUsMoney() : request.WorkerSalary.Value.ToUsMoney(),
            AgencyId = agencyId
        });

        request.InvitationSentItAt = now;
        await repository.Update(request);
        await repository.SaveChangesAsync();
        return Ok();
    }
}