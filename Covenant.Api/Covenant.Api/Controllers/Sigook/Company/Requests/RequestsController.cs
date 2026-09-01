using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Company.Requests;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Company)]
[ServiceFilter(typeof(CompanyIdFilter))]
public class RequestsController(IRequestService requestService) : ControllerBase
{
    public const string RouteName = "api/company/requests";

    /// <summary>Gets the paginated list of requests belonging to the current company.</summary>
    /// <param name="repository">Request repository resolved from DI.</param>
    /// <param name="filter">Filter and pagination criteria.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RequestListModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] IRequestRepository repository, GetRequestForCompanyFilter filter)
    {
        if (User.IsCompanyUser())
        {
            filter.CompanyUserId = User.GetUserId();
        }
        var result = await repository.GetRequestsForCompany(User.GetCompanyId(), filter);
        return Ok(result);
    }

    /// <summary>Gets the detail of a specific company request by its identifier.</summary>
    /// <param name="repository">Request repository resolved from DI.</param>
    /// <param name="id">Request identifier.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompanyRequestDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromServices] IRequestRepository repository, [FromRoute] Guid id) =>
        this.GetByIdResult(await repository.GetRequestDetailForCompany(id));

    /// <summary>Creates a new request for the current company.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyRequestDetailModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] RequestCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        Result<Guid> result = await requestService.CompanyCreateRequest(model);
        if (result) return CreatedAtAction(nameof(GetById), new { id = result.Value },
            new CompanyRequestDetailModel { Id = result.Value });
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Updates the requirements of an existing request.</summary>
    /// <param name="id">Request identifier.</param>
    /// <param name="model">Updated requirements.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] RequestUpdateRequirementsModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        Result result = await requestService.UpdateRequirements(id, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Generates and downloads the Excel timesheet report for a request of the current company.</summary>
    /// <param name="timesheetService">Timesheet service resolved from DI.</param>
    /// <param name="id">Request identifier.</param>
    [HttpGet("{id}/TimeSheets/File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTimeSheetsFile([FromServices] ITimesheetService timesheetService, [FromRoute] Guid id)
    {
        var result = await timesheetService.GetCompanyRequestTimesheetFile(id, User.GetCompanyId());
        if (!result) return NotFound();
        return File(result.Value.Document.ToArray(), CovenantConstants.ExcelMime, result.Value.DocumentName);
    }

    /// <summary>Cancels an existing request.</summary>
    /// <param name="id">Request identifier.</param>
    /// <param name="model">Cancellation detail.</param>
    [HttpPut("{id}/Cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Cancel([FromRoute] Guid id, [FromBody] RequestCancellationDetailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await requestService.CancelRequest(id, model);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}
