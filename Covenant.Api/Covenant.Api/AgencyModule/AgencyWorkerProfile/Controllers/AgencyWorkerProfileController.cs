using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyWorkerProfile.Controllers;

[Route(RouteName)]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class AgencyWorkerProfileController : Controller
{
    public const string RouteName = "api/AgencyWorkerProfile";
    private IMediator mediator;

    private readonly IWorkerRepository _workerRepository;
    private readonly IWorkerService workerService;
    private readonly IAgencyService agencyService;

    public AgencyWorkerProfileController(
        IWorkerRepository workerRepository,
        IWorkerService workerService,
        IAgencyService agencyService)
    {
        _workerRepository = workerRepository;
        this.workerService = workerService;
        this.agencyService = agencyService;
    }

    protected IMediator Mediator => mediator ?? (mediator = HttpContext.RequestServices.GetService<IMediator>());

    /// <summary>Gets a paginated list of worker profiles for the current agency.</summary>
    /// <param name="filter">Worker profile filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WorkerProfileListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] GetWorkerProfileFilter filter) =>
        Ok(await _workerRepository.GetWorkersProfile(User.GetAgencyId(), filter));

    /// <summary>Generates and downloads an Excel report of the current agency's worker profiles.</summary>
    /// <param name="filter">Worker profile filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile([FromQuery] GetWorkerProfileFilter filter)
    {
        var data = _workerRepository.GetAllWorkersProfile(User.GetAgencyId(), filter).ToList();
        var file = await Mediator.Send(new GenerateAgencyWorkersProfileReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }

    /// <summary>Creates a new worker profile, optionally associated with a request.</summary>
    /// <param name="requestId">Optional request identifier to associate the new worker with.</param>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWorkerProfile([FromQuery] int? requestId)
    {
        var result = await workerService.CreateWorker(requestId);
        if (result)
        {
            return Ok(result.Value);
        }
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Gets the detail of a worker profile by its identifier.</summary>
    /// <param name="id">Identifier of the worker profile.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkerProfileDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var detail = await _workerRepository.GetWorkerProfileDetail(wp => wp.Id == id);
        if (detail is null) return NotFound();
        return Ok(detail);
    }

    /// <summary>Gets the other documents of the specified worker profile.</summary>
    /// <param name="id">Identifier of the worker profile.</param>
    [HttpGet("{id:guid}/OtherDocument")]
    [ProducesResponseType(typeof(List<CovenantFileModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> OtherDocument(Guid id) =>
        Ok(await _workerRepository.GetOtherDocuments(id));

    /// <summary>Marks a worker profile as Do Not Use (DNU).</summary>
    /// <param name="id">Identifier of the worker profile.</param>
    [HttpPut("{id:guid}/Dnu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Dnu(Guid id)
    {
        Guid agencyId = User.GetAgencyId();
        var entity = await _workerRepository.GetProfile(w => w.Id == id && w.AgencyId == agencyId);
        if (entity is null) return BadRequest();
        entity.UpdateDnu();
        await _workerRepository.UpdateProfile(entity);
        await _workerRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Toggles the contractor flag of a worker profile.</summary>
    /// <param name="service">Time service providing the current date.</param>
    /// <param name="id">Identifier of the worker profile.</param>
    [HttpPut("{id:guid}/IsContractor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IsContractor([FromServices] ITimeService service, Guid id)
    {
        Guid agencyId = User.GetAgencyId();
        var entity = await _workerRepository.GetProfile(w => w.Id == id && w.AgencyId == agencyId);
        if (entity is null) return BadRequest();
        var result = entity.UpdateContractor(service.GetCurrentDateTime());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await _workerRepository.UpdateProfile(entity);
        await _workerRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Toggles the subcontractor flag of a worker profile.</summary>
    /// <param name="service">Time service providing the current date.</param>
    /// <param name="id">Identifier of the worker profile.</param>
    [HttpPut("{id:guid}/IsSubcontractor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IsSubcontractor([FromServices] ITimeService service, Guid id)
    {
        Guid agencyId = User.GetAgencyId();
        var entity = await _workerRepository.GetProfile(w => w.Id == id && w.AgencyId == agencyId);
        if (entity is null) return BadRequest();
        var result = entity.UpdateSubcontractor(service.GetCurrentDateTime());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await _workerRepository.UpdateProfile(entity);
        await _workerRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Updates the email address of the user linked to a worker profile.</summary>
    /// <param name="service">Identity server service used to update the user email.</param>
    /// <param name="workerProfileId">Identifier of the worker profile.</param>
    /// <param name="model">New email information.</param>
    [HttpPut("{workerProfileId:guid}/Email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Email([FromServices] IIdentityServerService service, [FromRoute] Guid workerProfileId, [FromBody] UpdateEmailModel model)
    {
        Guid agencyId = User.GetAgencyId();
        var profile = await _workerRepository.GetProfile(c => c.Id == workerProfileId && c.AgencyId == agencyId);
        if (profile is null) return BadRequest();
        var rUpdate = await service.UpdateUserEmail(new UpdateEmailModel(profile.WorkerId) { NewEmail = model?.NewEmail });
        if (!rUpdate) return BadRequest(ModelState.AddErrors(rUpdate.Errors));
        return Ok();
    }

    /// <summary>Marks a worker profile as approved to work.</summary>
    /// <param name="timeService">Time service providing the current date.</param>
    /// <param name="id">Identifier of the worker profile.</param>
    [HttpPut("{id}/ApprovedToWork")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateApprovedToWork([FromServices] ITimeService timeService, Guid id)
    {
        var entity = await _workerRepository.GetProfile(p => p.Id == id);
        if (entity is null || entity.AgencyId != User.GetAgencyId()) return BadRequest();
        Result result = entity.UpdateApprovedToWork(timeService.GetCurrentDateTime());
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        await _workerRepository.UpdateProfile(entity);
        await _workerRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Updates the external identifier of a worker profile.</summary>
    /// <param name="id">Identifier of the worker profile.</param>
    /// <param name="model">Model containing the new external identifier.</param>
    [HttpPut("{id:guid}/ExternalId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExternalId(Guid id, [FromBody] WorkerProfileDetailModel model)
    {
        var entity = await _workerRepository.GetProfile(w => w.Id == id);
        if (entity is null) return NotFound();
        entity.ExternalId = model?.ExternalId;
        await _workerRepository.UpdateProfile(entity);
        await _workerRepository.SaveChangesAsync();
        return Ok();
    }

    /// <summary>Updates the tax category of a worker profile.</summary>
    /// <param name="id">Identifier of the worker profile.</param>
    /// <param name="model">Model containing the new tax category.</param>
    [HttpPut("{id}/tax-category")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTaxCategory([FromRoute] Guid id, [FromBody] WorkerProfileDetailModel model)
    {
        await agencyService.UpdateWorkerProfileTaxCategory(id, model);
        return Ok();
    }

    /// <summary>Updates the tax rate of a worker profile.</summary>
    /// <param name="id">Identifier of the worker profile.</param>
    /// <param name="model">Model containing the new tax rate.</param>
    [HttpPut("{id}/tax-rate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTaxRate([FromRoute] Guid id, [FromBody] WorkerProfileDetailModel model)
    {
        await agencyService.UpdateWorkerProfileTaxRate(id, model);
        return Ok();
    }

    /// <summary>Gets worker profiles for a dropdown filtered by a search term.</summary>
    /// <param name="searchTerm">Term used to filter worker profiles.</param>
    [HttpGet("Dropdown")]
    [ProducesResponseType(typeof(List<AgencyWorkerDropdownModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllWorkerProfiles([FromQuery] string searchTerm)
    {
        var data = await _workerRepository.GetWorkerProfilesDropdown(User.GetAgencyIds(), searchTerm);
        return Ok(data);
    }
}