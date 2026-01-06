using Covenant.Api.Authorization;
using Covenant.Api.Shared.WorkerComment.Models;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid workerId, Pagination pagination) =>
            Ok(await _commentsRepository.GetComments(workerId, pagination ?? new Pagination()));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid workerId, Guid id)
        {
            var entity = await _commentsRepository.GetCommentModel(workerId, id);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        [Obsolete]
        [HttpPost]
        [Route("Agency")]
        [Authorize(Policy = PolicyConfiguration.Agency)]
        public async Task<IActionResult> PostAgency(Guid workerId, [FromBody] CreateCommentModel model) =>
            await CreateComment(workerId, _agencyRepository.AgencyExists, agencyId => Covenant.Common.Entities.Worker.WorkerComment.CommentPostByAgency(workerId, agencyId, model.Comment, model.Rate));

        [Obsolete]
        [HttpPost]
        [Route("Company")]
        [Authorize(Policy = PolicyConfiguration.Company)]
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

        [HttpPut("{id}")]
        [Authorize]
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