using Covenant.Api.Utils.Extensions;
using Covenant.Common.Models.Notification;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook;

[ApiController]
[Route(RouteName)]
public class EmailPreferencesController(IWorkerService workerService) : ControllerBase
{
    public const string RouteName = "api/EmailPreferences";

    /// <summary>Unsubscribes the owner of an email address from a given notification type.</summary>
    /// <param name="model">Unsubscribe request data.</param>
    [AllowAnonymous]
    [HttpPost("Unsubscribe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await workerService.Unsubscribe(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }
}
