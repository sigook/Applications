using Covenant.Api.Authorization;
using Covenant.Common.Models.Worker;
using Covenant.Api.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
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
        public const string RouteName = "api/CompanyWorker/{workerProfileId:guid}/Comment";

        /// <summary>Creates a comment and rating for a worker on behalf of the current company.</summary>
        /// <param name="workerProfileId">Identifier of the worker profile the comment is about.</param>
        /// <param name="workerService">Worker service.</param>
        /// <param name="model">Comment content and rating.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostCompany([FromRoute] Guid workerProfileId,
            [FromServices] IWorkerService workerService,
            [FromBody] CreateCommentModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await workerService.AddCompanyComment(workerProfileId, model.Comment, model.Rate);
            if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
            return Ok();
        }
    }
}
