using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        public async Task<ActionResult> Get(Guid workerProfileId, Pagination pagination) =>
            Ok(await _requestRepository.GetRequestsHistoryByWorkerProfileId(workerProfileId, pagination));
    }
}