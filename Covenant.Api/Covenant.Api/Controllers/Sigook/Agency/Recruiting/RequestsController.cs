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

namespace Covenant.Api.Controllers.Sigook.Agency.Recruiting;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Recruiting)]
[ServiceFilter(typeof(AgencyIdFilter))]
[ServiceFilter(typeof(AgencyPersonnelIdFilter))]
public class RequestsController(IMediator mediator, IRequestService requestService, IRequestRepository requestRepository) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/requests";

    /// <summary>Gets a paginated list of requests for the current agency along with the job boards summary aligned with the same filter.</summary>
    /// <param name="pagination">Request filter and pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(AgencyRequestsPagedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(GetRequestForAgencyFilter pagination)
    {
        var agencyId = User.GetAgencyId();
        if (pagination.OnlyMine) pagination.Recruiter = User.GetNickname();
        if (pagination.AgencyId.HasValue) agencyId = pagination.AgencyId.Value;
        pagination.HasPermissionToSeeInternalRequests = User.IsAccountingManager();
        pagination.SalesPersonnelId = null;
        return Ok(await requestService.GetRequestsForAgency(agencyId, pagination));
    }

    /// <summary>Gets all requests for the current agency without pagination.</summary>
    /// <param name="pagination">Request filter parameters.</param>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<AgencyRequestListModel>), StatusCodes.Status200OK)]
    public IActionResult GetAll(GetRequestForAgencyFilter pagination)
    {
        pagination.SalesPersonnelId = null;
        var data = requestRepository.GetAllRequestsForAgency(User.GetAgencyId(), pagination).ToList();
        return Ok(data);
    }

    /// <summary>Generates and downloads an Excel report of the current agency's requests.</summary>
    /// <param name="pagination">Request filter parameters.</param>
    [HttpGet("File")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile(GetRequestForAgencyFilter pagination)
    {
        if (pagination.OnlyMine)
        {
            pagination.Recruiter = User.GetNickname();
        }
        pagination.HasPermissionToSeeInternalRequests = User.IsAccountingManager();
        pagination.SalesPersonnelId = null;
        var data = requestRepository.GetAllRequestsForAgency(User.GetAgencyId(), pagination).ToList();
        var file = await mediator.Send(new GenerateAgencyRequestsReport(data));
        return File(file.Document.ToArray(), CovenantConstants.ExcelMime, file.DocumentName);
    }
}
