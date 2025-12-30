using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyWorkerProfileNote
{
    [Route(RouteName)]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyWorkerProfileNoteController : Controller
    {
        public const string RouteName = "api/AgencyWorkerProfile/{workerProfileId}/Note";
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] IWorkerRepository repository, [FromRoute] Guid workerProfileId, [FromBody] WorkerProfileNoteCreateModel model)
        {
            string createdBy = User.GetNickname();
            var result = WorkerProfileNote.Create(workerProfileId, model?.Note, createdBy);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            await repository.Create(result.Value);
            await repository.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IWorkerRepository repository, [FromRoute] Guid workerProfileId, Pagination pagination) =>
            Ok(await repository.GetWorkerProfileNotes(workerProfileId, pagination));
    }
}