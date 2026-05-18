using Covenant.Api.Authorization;
using Covenant.Api.Shared.WorkerComment.Models;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Shared.WorkerComment.Controllers
{
    [Route("api/worker/{workerId}/comment")]
    [ApiController]
    [Produces("application/json")]
    public class WorkerCommentController : ControllerBase
    {
        private readonly IWorkerCommentsRepository _commentsRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IUserRepository _userRepository;

        public WorkerCommentController(
            IWorkerCommentsRepository commentsRepository,
            IAgencyRepository agencyRepository,
            IUserRepository userRepository)
        {
            _commentsRepository = commentsRepository;
            _agencyRepository = agencyRepository;
            _userRepository = userRepository;
        }

        /// <summary>Gets the paginated comments for a worker.</summary>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="pagination">Pagination criteria.</param>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedList<WorkerCommentModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid workerId, Pagination pagination) =>
            Ok(await _commentsRepository.GetComments(workerId, pagination ?? new Pagination()));

        /// <summary>Gets a single worker comment by id.</summary>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="id">Comment identifier.</param>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(WorkerCommentModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid workerId, Guid id)
        {
            var entity = await _commentsRepository.GetCommentModel(workerId, id);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        /// <summary>Creates a worker comment posted by the current agency.</summary>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="model">Comment data.</param>
        [Obsolete]
        [HttpPost]
        [Route("Agency")]
        [Authorize(Policy = PolicyConfiguration.Agency)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAgency(Guid workerId, [FromBody] CreateCommentModel model) =>
            await CreateComment(workerId, _agencyRepository.AgencyExists, agencyId => Covenant.Common.Entities.Worker.WorkerComment.CommentPostByAgency(workerId, agencyId, model.Comment, model.Rate));

        /// <summary>Creates a worker comment posted by the current company.</summary>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="model">Comment data.</param>
        [Obsolete]
        [HttpPost]
        [Route("Company")]
        [Authorize(Policy = PolicyConfiguration.Company)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostCompany(Guid workerId, [FromBody] CreateCommentModel model) =>
            await CreateComment(workerId, _userRepository.UserIsCompany, companyId => Covenant.Common.Entities.Worker.WorkerComment.CommentPostByCompany(workerId, companyId, model.Comment, model.Rate));

        private async Task<IActionResult> CreateComment(Guid workerId, Func<Guid, Task<bool>> isValidUser, Func<Guid, Covenant.Common.Entities.Worker.WorkerComment> buildEntity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Guid userId = User.GetUserId();
            if (!await isValidUser(userId)) return BadRequest(ModelState.AddError("Unauthorized user"));
            if (!await _userRepository.UserIsWorker(workerId)) return BadRequest("Worker Not Found");
            var entity = buildEntity(userId);
            await _commentsRepository.CreateComment(entity);
            return CreatedAtAction(nameof(GetById), new { workerId, entity.Id }, new { });
        }

        /// <summary>Updates an existing worker comment.</summary>
        /// <param name="workerId">Worker identifier.</param>
        /// <param name="id">Comment identifier.</param>
        /// <param name="model">Updated comment data.</param>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(Guid workerId, Guid id, [FromBody] CreateCommentModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _commentsRepository.GetComment(workerId, id);
            if (entity is null) return BadRequest();
            await _commentsRepository.UpdateComment(entity.Update(model.Comment, model.Rate));
            return Ok();
        }
    }
}