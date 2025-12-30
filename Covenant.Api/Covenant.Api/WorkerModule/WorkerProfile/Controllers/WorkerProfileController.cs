using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerProfile.Controllers
{
    [Route("api/WorkerProfile")]
    [Authorize(Policy = PolicyConfiguration.Worker)]
    [ApiController]
    [Produces("application/json")]
    public class WorkerProfileController : ControllerBase
    {
        private readonly IWorkerService workerService;

        public WorkerProfileController(IWorkerService workerService)
        {
            this.workerService = workerService;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromServices] IWorkerRepository repository) => Ok(await repository.GetProfiles(User.GetUserId()));

        [HttpPost]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Post([FromQuery] int? orderId)
        {
            var result = await workerService.CreateWorker(orderId);
            if (result)
            {
                return Ok(result.Value);
            }
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpGet("{profileId}")]
        public async Task<ActionResult> GetById([FromServices] IWorkerRepository repository, Guid profileId)
        {
            var model = await repository.GetWorkerProfileDetail(profileId);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpGet("{profileId}/BasicInfo")]
        public async Task<ActionResult> GetBasicInfo([FromServices] IWorkerRepository repository, Guid profileId)
        {
            WorkerProfileBasicInfoModel model = await repository.GetWorkerProfileBasicInfo(profileId);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpGet("{profileId:guid}/OtherDocument")]
        public async Task<IActionResult> OtherDocument([FromServices] IWorkerRepository repository, Guid profileId) =>
            Ok(await repository.GetOtherDocuments(profileId));
    }
}