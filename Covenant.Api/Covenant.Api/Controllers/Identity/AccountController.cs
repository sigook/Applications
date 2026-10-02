using Covenant.Api.Configuration;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Covenant.Core.BL.Services.Identity;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;

namespace Covenant.Api.Controllers.Identity;

[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("Account")]
public class AccountController(
    IUserAdministrationService userAdministrationService,
    IPasswordResetService passwordResetService,
    IAccountNotificationService notifications,
    IValidator<ConfirmEmailAddressModel> confirmEmailValidator,
    IValidator<CreatePasswordModel> createPasswordValidator,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("ConfirmEmailAddress")]
    public IActionResult ConfirmEmailAddress([FromQuery] string token, [FromQuery] string id) => RedirectToWeb("confirm-email", token, id);

    [HttpGet("CreatePassword"), HttpGet("ResetPassword")]
    public IActionResult CreatePassword([FromQuery] string token, [FromQuery] string id) => RedirectToWeb("create-password", token, id);

    [HttpPost("ConfirmEmail")]
    [EnableRateLimiting(RateLimitingConfiguration.PasswordResetPolicy)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailAddressModel model)
    {
        var validation = await confirmEmailValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(AddErrors(validation));

        var result = await userAdministrationService.ConfirmEmail(model.Id, model.Token);
        if (result.IsSuccess) return Ok();
        return BadRequest(new { error = PasswordResetService.InvalidToken });
    }

    [HttpPost("CreatePassword")]
    [EnableRateLimiting(RateLimitingConfiguration.PasswordResetPolicy)]
    public async Task<IActionResult> CreatePassword([FromBody] CreatePasswordModel model)
    {
        var validation = await createPasswordValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(AddErrors(validation));

        var result = await passwordResetService.CreatePassword(model.Id, model.Token, model.Password);
        if (result.Succeeded) return Ok();
        return BadRequest(new { error = result.Error, messages = result.Messages });
    }

    [HttpPost("ResendConfirmationLink")]
    [EnableRateLimiting(RateLimitingConfiguration.PasswordResetPolicy)]
    public async Task<IActionResult> ResendConfirmationLink([FromQuery] string userName)
    {
        if (string.IsNullOrEmpty(userName)) return BadRequest();
        await notifications.ResendConfirmAccount(userName);
        return Ok();
    }

    private RedirectResult RedirectToWeb(string path, string token, string id) =>
        Redirect($"{configuration.GetWebClientUrl()?.TrimEnd('/')}/{path}?token={Uri.EscapeDataString(token ?? string.Empty)}&id={Uri.EscapeDataString(id ?? string.Empty)}");

    private ModelStateDictionary AddErrors(ValidationResult validation)
    {
        foreach (var error in validation.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return ModelState;
    }
}
