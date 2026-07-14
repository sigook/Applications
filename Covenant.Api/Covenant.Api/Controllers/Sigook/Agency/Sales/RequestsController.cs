using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Sales;

[Route("api/agency/sales/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Sales)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class RequestsController(IMediator mediator, IRequestService requestService) : ControllerBase
{
    private Guid? SalesScope => User.IsSales() ? User.GetUserId() : null;

    /// <summary>Gets a paginated list of requests for the sales module. Sales users only see requests where they are the assigned sales representative.</summary>
    /// <param name="pagination">Request filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyRequestsPagedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetRequestForAgencyFilter pagination)
    {
        var agencyId = User.GetAgencyId();
        if (pagination.AgencyId.HasValue) agencyId = pagination.AgencyId.Value;
        pagination.HasPermissionToSeeInternalRequests = User.IsAccountingManager();
        pagination.SalesUserId = SalesScope;
        return Ok(await requestService.GetRequestsForAgency(agencyId, pagination));
    }

    /// <summary>Generates and downloads an Excel report of the sales module requests.</summary>
    /// <param name="repository">Request repository.</param>
    /// <param name="pagination">Request filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile([FromServices] IRequestRepository repository, GetRequestForAgencyFilter pagination)
    {
        pagination.HasPermissionToSeeInternalRequests = User.IsAccountingManager();
        pagination.SalesUserId = SalesScope;
        var data = repository.GetAllRequestsForAgency(User.GetAgencyId(), pagination).ToList();
        var file = await mediator.Send(new GenerateAgencyRequestsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
