using Covenant.Api.Authorization;
using Covenant.Api.Shared.WorkerComment.Models;
using Covenant.Api.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Workers;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Agency)]
[ServiceFilter(typeof(AgencyIdFilter))]
public class CommentsController : ControllerBase
{
    public const string RouteName = "api/agency/workers/{workerProfileId:guid}/Comments";

    /// <summary>Posts a comment about a worker on behalf of the current agency.</summary>
    /// <param name="workerProfileId">Identifier of the worker profile the comment is about.</param>
    /// <param name="workerService">Worker service.</param>
    /// <param name="model">Comment content and rating.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid workerProfileId,
        [FromServices] IWorkerService workerService,
        [FromBody] CreateCommentModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await workerService.AddAgencyComment(workerProfileId, model.Comment, model.Rate);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
