using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.ManagerModule.WorkerProfilePunchCardId
{
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Covenant)]
    [Route("api/WorkerProfile/{profileId}/PunchCardId")]
    public class WorkerProfilePunchCardIdController : ControllerBase
    {
        private readonly IWorkerService workerService;
        private readonly IWorkerRepository _workerRepository;

        public WorkerProfilePunchCardIdController(IWorkerService workerService, IWorkerRepository workerRepository)
        {
            this.workerService = workerService;
            _workerRepository = workerRepository;
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid profileId, [FromBody] WorkerProfilePunchCardIdUpdateModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            User.TryGetUserId(out Guid agencyId);
            var result = await workerService.UpdateWorkerPunchCardId(profileId, agencyId, model.PunchCardId);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid profileId)
        {
            var model = await _workerRepository.GetWorkerProfilePunchCarId(profileId);
            if (model is null) NotFound();
            return Ok(model);
        }
    }
}