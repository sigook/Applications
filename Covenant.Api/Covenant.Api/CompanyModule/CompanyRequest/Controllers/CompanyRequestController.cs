using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.CompanyModule.CompanyRequest.Controllers
{
    [Route(RouteName)]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyConfiguration.Company)]
    [ServiceFilter(typeof(CompanyIdFilter))]
    public class CompanyRequestController : ControllerBase
    {
        public const string RouteName = "api/CompanyRequest";
        private readonly IRequestService requestService;

        public CompanyRequestController(IRequestService requestService)
        {
            this.requestService = requestService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get([FromServices] IRequestRepository repository, GetRequestForCompanyFilter filter)
        {
            if (User.IsCompanyUser())
            {
                filter.CompanyUserId = User.GetUserId();
            }
            var result = await repository.GetRequestsForCompany(User.GetCompanyId(), filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById([FromServices] IRequestRepository repository, [FromRoute] Guid id) =>
            this.GetByIdResult(await repository.GetRequestDetailForCompany(id));

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Post([FromBody] RequestCreateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Result<Guid> result = await requestService.CompanyCreateRequest(model);
            if (result) return CreatedAtAction(nameof(GetById), new { id = result.Value },
                new CompanyRequestDetailModel { Id = result.Value });
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] RequestUpdateRequirementsModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Result result = await requestService.UpdateRequirements(id, model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }

        [HttpPut]
        [Route("{id}/Cancel")]
        public async Task<ActionResult> Cancel([FromRoute] Guid id, [FromBody] RequestCancellationDetailModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await requestService.CancelRequest(id, model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}