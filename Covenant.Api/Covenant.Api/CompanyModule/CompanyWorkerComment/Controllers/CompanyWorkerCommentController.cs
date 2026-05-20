using Covenant.Api.Authorization;
using Covenant.Api.Shared.WorkerComment.Models;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyWorkerComment.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyWorkerCommentController : ControllerBase
    {
        public const string RouteName = "api/CompanyWorker/{workerId:guid}/Comment";

        /// <summary>Creates a comment and rating for a worker on behalf of the current company.</summary>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="commentsRepository">Worker comments repository resolved from DI.</param>
        /// <param name="userRepository">User repository resolved from DI.</param>
        /// <param name="model">Comment content and rating.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostCompany([FromRoute] Guid workerId, [FromServices] IWorkerCommentsRepository commentsRepository, [FromServices] IUserRepository userRepository, [FromBody] CreateCommentModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await userRepository.UserIsWorker(workerId)) return BadRequest("Worker not found");
            Guid companyId = User.GetCompanyId();
            var entity = WorkerComment.CommentPostByCompany(workerId, companyId, model.Comment, model.Rate);
            await commentsRepository.CreateComment(entity);
            return CreatedAtAction("GetById", "WorkerComment", new { workerId, entity.Id }, new { });
        }
    }
}
