using Covenant.IdentityServer.Controllers.Account.Models;
using Covenant.IdentityServer.Models;
using Covenant.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.IdentityServer.Controllers.Account;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class PasswordController : ControllerBase
{
    private readonly IPasswordResetService _passwordResetService;

    public PasswordController(IPasswordResetService passwordResetService)
    {
        _passwordResetService = passwordResetService;
    }

    [HttpPost("forgot")]
    public async Task<IActionResult> Forgot([FromBody] ForgotPasswordModel model)
    {
        await _passwordResetService.RequestCodeAsync(model.Email);
        return Accepted();
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset([FromBody] ResetPasswordWithCodeModel model)
    {
        PasswordResetResult result = await _passwordResetService.ResetPasswordAsync(model.Email, model.Code, model.NewPassword);
        if (result.Succeeded) return Ok();
        return BadRequest(new { error = result.Error, messages = result.Messages });
    }
}
