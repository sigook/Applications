using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agency.Recruiting.Workers;

[Route(RouteName)]
[ApiController]
[Produces("application/json")]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class RequestHistoryController(IRequestRepository requestRepository) : ControllerBase
{
    public const string RouteName = "api/agency/recruiting/workers/{workerProfileId}/requesthistory";

    /// <summary>Gets a paginated request history for the specified worker profile.</summary>
    /// <param name="workerProfileId">Identifier of the worker profile.</param>
    /// <param name="pagination">Pagination parameters.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RequestListModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Get(Guid workerProfileId, Pagination pagination) =>
        Ok(await requestRepository.GetRequestsHistoryByWorkerProfileId(workerProfileId, pagination));
}
