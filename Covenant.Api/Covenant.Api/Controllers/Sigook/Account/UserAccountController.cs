using Covenant.Api.Utils.Extensions;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Account;

[ApiController]
[Authorize]
[Route(RouteName)]
public class UserAccountController(
    IUserAccountService userAccountService,
    IUserRepository userRepository) : ControllerBase
{
    public const string RouteName = "api/Account";
    public const string DeactivateRoute = "~/identity";

    /// <summary>Changes the email address of the current authenticated user.</summary>
    /// <param name="model">New email data.</param>
    [HttpPost("ChangeEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel model)
    {
        var result = await userAccountService.ChangeEmail(model);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok();
    }

    /// <summary>Gets the email address of the current authenticated user.</summary>
    [HttpGet("GetEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmail()
    {
        User.TryGetUserId(out Guid userId);
        string email = await userRepository.GetUserEmail(userId);
        return Ok(new { Email = email });
    }

    /// <summary>Deactivates the current authenticated user account.</summary>
    [HttpPatch(DeactivateRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InactiveAccount()
    {
        if (!User.TryGetUserId(out Guid id)) return Unauthorized();
        var result = await userAccountService.InactiveUser(id);
        if (!result) return BadRequest(result.Errors);
        return Ok();
    }
}
