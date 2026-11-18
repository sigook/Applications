using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Teams;
using Covenant.Common.Utils.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Agencies;

[Route(RouteName)]
[ApiController]
[Authorize(Policy = PolicyConfiguration.Admin)]
public class TeamsController(ITeamsService teamsService, IValidator<TeamsChannelFileFilter> filterValidator) : ControllerBase
{
    public const string RouteName = "api/agency/teams";

    /// <summary>Searches the files of a private or shared Teams channel (its own SharePoint site, team-channel) whose name or content matches Name, most recently modified first.</summary>
    [HttpGet("files")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyList<TeamsChannelFileModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindFiles([FromQuery] TeamsChannelFileFilter filter)
    {
        var validation = await filterValidator.ValidateAsync(filter);
        if (!validation.IsValid) return BadRequest(ModelState.AddErrors(validation.ToResultFailure().Errors));
        var result = await teamsService.FindChannelFiles(filter);
        if (result) return Ok(result.Value);
        return result.IsNotFound ? NotFound() : BadRequest(ModelState.AddErrors(result.Errors));
    }

    /// <summary>Downloads the file of a private or shared Teams channel whose name is exactly Name.</summary>
    [HttpGet("files/content")]
    [Produces("application/octet-stream", "application/json")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFileContent([FromQuery] TeamsChannelFileFilter filter)
    {
        var validation = await filterValidator.ValidateAsync(filter);
        if (!validation.IsValid) return BadRequest(ModelState.AddErrors(validation.ToResultFailure().Errors));
        var result = await teamsService.GetChannelFile(filter);
        if (result) return File(result.Value, "application/octet-stream", filter.Name.Trim());
        return result.IsNotFound ? NotFound() : BadRequest(ModelState.AddErrors(result.Errors));
    }
}
