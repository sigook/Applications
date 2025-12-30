using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.WorkerModule.WorkerProfile.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/WorkerProfile/{profileId}/JobExperience")]
    public class WorkerProfileJobExperienceController : ControllerBase
    {
        private readonly IWorkerRepository _workerRepository;
        public WorkerProfileJobExperienceController(IWorkerRepository workerRepository) => _workerRepository = workerRepository;

        [HttpPost]
        public async Task<IActionResult> Post(Guid profileId, [FromBody] WorkerProfileJobExperienceModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _workerRepository.GetProfile(p => p.Id == profileId);
            if (entity is null) return BadRequest();
            var result = entity.AddJobExperience(model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid profileId, Guid id, [FromBody] WorkerProfileJobExperienceModel model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _workerRepository.GetProfile(p => p.Id == profileId);
            if (entity is null) return BadRequest();
            Result result = entity.UpdateJobExperience(id, model);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid profileId, Guid id)
        {
            var entity = await _workerRepository.GetProfile(p => p.Id == profileId);
            if (entity is null) return BadRequest();
            Result result = entity.DeleteJobExperience(id);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await _workerRepository.UpdateProfile(entity);
            await _workerRepository.SaveChangesAsync();
            return Ok();
        }
    }
}