using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyRequestWorker.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyRequestWorkerController : ControllerBase
    {
        public const string RouteName = "api/CompanyRequest/{requestId}/Worker";
        private readonly ICompanyService companyService;
        private readonly IRequestService requestService;
        private readonly IRequestRepository _requestRepository;

        public CompanyRequestWorkerController(
            ICompanyService companyService,
            IRequestService requestService,
            IRequestRepository requestRepository)
        {
            this.companyService = companyService;
            this.requestService = requestService;
            _requestRepository = requestRepository;
        }

        /// <summary>Gets the paginated list of workers associated with a request.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="filter">Filter and pagination criteria.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<AgencyWorkerRequestModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromRoute] Guid requestId, [FromQuery] GetWorkersRequestFilter filter)
        {
            return Ok(await _requestRepository.GetWorkersRequestByRequestId(requestId, filter));
        }

        /// <summary>Gets the detail of a specific worker assigned to a request.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="workerId">Worker identifier.</param>
        [HttpGet("{workerId}")]
        [ProducesResponseType(typeof(AgencyWorkerRequestModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid requestId, Guid workerId)
        {
            AgencyWorkerRequestModel model = await _requestRepository.GetRequestWorkerByCompanyId(User.GetCompanyId(), requestId, workerId);
            if (model is null) return NotFound();
            return Ok(model);
        }

        /// <summary>Requests a new worker for the specified request.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="model">Comment associated with the request.</param>
        [HttpPost]
        [Route("RequestNewWorker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid requestId, [FromBody] CommentsModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await companyService.RequestNewWorker(requestId, model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        /// <summary>Rejects a worker assigned to the specified request.</summary>
        /// <param name="requestId">Request identifier.</param>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="model">Comment explaining the rejection.</param>
        [HttpPut("{workerId}/Reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Reject([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] CommentsModel model)
        {
            var result = await requestService.RejectWorker(requestId, workerId, model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}