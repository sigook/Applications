using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Covenant.Api.Controllers.Identity;

[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("Password")]
public class PasswordController(
    IPasswordResetService passwordResetService,
    IValidator<ForgotPasswordModel> forgotValidator,
    IValidator<ResetPasswordWithCodeModel> resetValidator) : ControllerBase
{
    [HttpPost("forgot")]
    public async Task<IActionResult> Forgot([FromBody] ForgotPasswordModel model)
    {
        var validation = await forgotValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(AddErrors(validation));

        await passwordResetService.RequestCode(model.Email);
        return Accepted();
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset([FromBody] ResetPasswordWithCodeModel model)
    {
        var validation = await resetValidator.ValidateAsync(model);
        if (!validation.IsValid) return BadRequest(AddErrors(validation));

        var result = await passwordResetService.ResetPassword(model.Email, model.Code, model.NewPassword);
        if (result.Succeeded) return Ok();
        return BadRequest(new { error = result.Error, messages = result.Messages });
    }

    private ModelStateDictionary AddErrors(ValidationResult validation)
    {
        foreach (var error in validation.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return ModelState;
    }
}
