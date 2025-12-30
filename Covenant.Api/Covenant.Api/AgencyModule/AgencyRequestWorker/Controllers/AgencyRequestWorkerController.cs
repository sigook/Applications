using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestWorker.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestWorkerController : ControllerBase
    {
        public const string RouteName = "api/AgencyRequest/{requestId}/Worker";
        private readonly IAgencyService agencyService;
        private readonly IRequestService requestService;

        public AgencyRequestWorkerController(IAgencyService agencyService, IRequestService requestService)
        {
            this.agencyService = agencyService;
            this.requestService = requestService;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromServices] IRequestRepository repository, [FromRoute] Guid requestId, [FromQuery] GetWorkersRequestFilter pagination) =>
            Ok(await repository.GetWorkersRequestByRequestId(requestId, pagination));

        [HttpGet]
        [Route("Whole")]
        [Route("All")]
        public async Task<ActionResult> GetAllWorkers([FromServices] IRequestRepository repository, [FromRoute] Guid requestId, [FromQuery] Pagination pagination, string filter = null)
        {
            Guid agencyId = User.GetAgencyId();
            var list = await repository.GetAllWorkersThatCanApplyToRequest(agencyId, requestId, pagination ?? new Pagination(), filter);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromServices] IRequestRepository repository, Guid requestId, Guid id)
        {
            Guid agencyId = User.GetAgencyId();
            var model = await repository.GetWorkerRequestByAgencyId(agencyId, requestId, id);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Put([FromServices] IWorkerRequestRepository repository, Guid requestId, Guid id, [FromBody] AgencyBookWorkerModel model)
        {
            if (!ModelState.IsValid || model?.StartWorking is null) return BadRequest();
            var entity = await repository.GetWorkerRequest(id);
            if (entity is null || entity.RequestId != requestId) return BadRequest();
            Result result = entity.UpdateStartWorking(model.StartWorking.GetValueOrDefault());
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await repository.UpdateWorkerRequest(entity);
            await repository.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{workerId:guid}/Book")]
        public async Task<ActionResult> Post([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] AgencyBookWorkerModel model)
        {
            var result = await agencyService.BookWorker(requestId, workerId, model);
            if (result) return Ok(new AgencyWorkerRequestModel { Id = result.Value });
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpPut("{workerId:guid}/Reject")]
        public async Task<ActionResult> Reject([FromRoute] Guid requestId, [FromRoute] Guid workerId, [FromBody] CommentsModel model)
        {
            var result = await requestService.RejectWorker(requestId, workerId, model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}