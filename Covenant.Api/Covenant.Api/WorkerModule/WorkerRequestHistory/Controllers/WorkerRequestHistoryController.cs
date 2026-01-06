using Covenant.Api.Authorization;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerRequestHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Worker)]
    public class WorkerRequestHistoryController : ControllerBase
    {
        private readonly IRequestRepository _requestRepository;

        public WorkerRequestHistoryController(IRequestRepository requestRepository) =>
            _requestRepository = requestRepository;

        [HttpGet]
        public async Task<ActionResult> Get(Pagination pagination) =>
            Ok(await _requestRepository.GetRequestsHistoryForWorker(User.GetUserId(), pagination));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _requestRepository.GetRequestDetailForWorker(User.GetUserId(), id);
            if (model is null) return NotFound();
            return Ok(model);
        }
    }
}