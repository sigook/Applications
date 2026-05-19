using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyWorkerProfileRequestHistory.Controllers
{
    [Route(RouteUrl)]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyWorkerProfileRequestHistoryController : ControllerBase
    {
        public const string RouteUrl = "api/AgencyWorkerProfile/{workerProfileId}/RequestHistory";
        private readonly IRequestRepository _requestRepository;

        public AgencyWorkerProfileRequestHistoryController(IRequestRepository requestRepository) =>
            _requestRepository = requestRepository;

        /// <summary>Gets a paginated request history for the specified worker profile.</summary>
        /// <param name="workerProfileId">Identifier of the worker profile.</param>
        /// <param name="pagination">Pagination parameters.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<RequestListModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(Guid workerProfileId, Pagination pagination) =>
            Ok(await _requestRepository.GetRequestsHistoryByWorkerProfileId(workerProfileId, pagination));
    }
}