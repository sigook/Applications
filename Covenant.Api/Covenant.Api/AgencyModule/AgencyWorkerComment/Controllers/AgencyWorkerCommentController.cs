using Covenant.Api.Authorization;
using Covenant.Api.Shared.WorkerComment.Models;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyWorkerComment.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyWorkerCommentController : ControllerBase
    {
        public const string RouteName = "api/AgencyWorker/{workerId:guid}/Comment";

        [HttpPost]
        public async Task<IActionResult> Post(Guid workerId,
            [FromServices] IWorkerCommentsRepository commentsRepository,
            [FromServices] IUserRepository userRepository,
            [FromBody] CreateCommentModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await userRepository.UserIsWorker(workerId)) return BadRequest("Worker not found");
            Guid agencyId = User.GetAgencyId();
            var entity = WorkerComment.CommentPostByAgency(workerId, agencyId, model.Comment, model.Rate);
            await commentsRepository.CreateComment(entity);
            return CreatedAtAction("GetById", "WorkerComment", new { workerId, entity.Id }, new { });
        }
    }
}