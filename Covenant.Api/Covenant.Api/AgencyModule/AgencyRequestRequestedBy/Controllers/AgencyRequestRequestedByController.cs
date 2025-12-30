using Covenant.Api.Authorization;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AgencyModule.AgencyRequestRequestedBy.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Authorize(Policy = PolicyConfiguration.Agency)]
    [ServiceFilter(typeof(AgencyIdFilter))]
    public class AgencyRequestRequestedByController : ControllerBase
    {
        private readonly IRequestRepository _repository;
        public const string RouteName = "api/AgencyRequest/{requestId}/RequestedBy";
        public AgencyRequestRequestedByController(IRequestRepository repository) => _repository = repository;

        [HttpPost("{contactPersonId}")]
        public async Task<IActionResult> Post(Guid requestId, Guid contactPersonId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _repository.GetRequestedBy(requestId, contactPersonId);
            if (entity != null) return BadRequest();
            entity = new RequestRequestedBy(requestId, contactPersonId);
            await _repository.Create(entity);
            await _repository.SaveChangesAsync();
            return CreatedAtAction("GetById", "AgencyRequestRequestedBy", new { requestId, contactPersonId }, new { });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid requestId, Pagination pagination) => Ok(await _repository.GetRequestedByList(requestId, pagination));

        [HttpGet("{contactPersonId}")]
        public async Task<IActionResult> GetById(Guid requestId, Guid contactPersonId)
        {
            var model = await _repository.GetRequestedByDetail(requestId, contactPersonId);
            if (model is null) return NotFound();
            return Ok(model);
        }

        [HttpDelete("{contactPersonId}")]
        public async Task<IActionResult> Delete(Guid requestId, Guid contactPersonId)
        {
            var entity = await _repository.GetRequestedBy(requestId, contactPersonId);
            if (entity is null) return BadRequest();
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}