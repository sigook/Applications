using Covenant.IdentityServer.Enums;
using Covenant.IdentityServer.Models;
using Covenant.IdentityServer.Models.Security;
using Covenant.IdentityServer.Configuration;
using Covenant.IdentityServer.Controllers.Account.Models;
using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Entities;
using Covenant.IdentityServer.Services;
using Covenant.IdentityServer.Views.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.Controllers.Account;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = Microsoft365OpenIdConnect.SchemeMicrosoft365Bearer)]
public class UserAdministrationController : ControllerBase
{
    private readonly UserManager<CovenantUser> _userManager;
    private readonly RoleManager<CovenantRole> _roleManager;
    private readonly ILogger<UserAdministrationController> _logger;
    private readonly IConfiguration configuration;
    private readonly IRazorViewToStringRenderer renderer;
    private readonly IEmailService emailService;

    public UserAdministrationController(
        UserManager<CovenantUser> userManager,
        RoleManager<CovenantRole> roleManager,
        ILogger<UserAdministrationController> logger,
        IConfiguration configuration,
        IRazorViewToStringRenderer renderer,
        IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        this.configuration = configuration;
        this.renderer = renderer;
        this.emailService = emailService;
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
    {
        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), $"Unknown role '{model.Role}'");
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            var newUser = new CovenantUser
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                UserName = model.Email,
            };
            newUser.EmailConfirmed = model.UserType switch
            {
                UserType.Agency or UserType.AgencyPersonnel => true,
                UserType.Company => string.IsNullOrEmpty(model.ConfirmPassword),
                _ => false
            };
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                var roles = new List<string> { model.Role };
                if (result.Succeeded)
                {
                    switch (model.UserType)
                    {
                        case UserType.Agency:
                            result = await _userManager.AddClaimAsync(newUser, Constants.ClaimAgencyId(model.AgencyId.Value));
                            break;
                        case UserType.AgencyPersonnel:
                            result = await _userManager.AddClaimAsync(newUser, Constants.ClaimAgencyId(model.AgencyId.Value));
                            await SendEmailToConfirmAndSetPassword(newUser);
                            break;
                        case UserType.CompanyUser:
                            result = await _userManager.AddClaimAsync(newUser, Constants.ClaimCompanyId(model.CompanyId.Value));
                            await SendEmailToConfirmAndSetPassword(newUser);
                            break;
                        case UserType.Company:
                            if (!newUser.EmailConfirmed)
                                await SendEmailToConfirm(newUser);
                            break;
                        case UserType.Worker:
                            if (!string.IsNullOrEmpty(model.ConfirmPassword))
                                await SendEmailToConfirm(newUser);
                            else
                                await SendEmailToConfirmAndSetPassword(newUser);
                            break;
                    }
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRolesAsync(newUser, roles);
                        if (result.Succeeded)
                        {
                            return Ok(new IdModel(newUser.Id));
                        }
                    }
                    return ReturnIdentityResultError(result);
                }
                return ReturnIdentityResultError(result);
            }
            return ReturnIdentityResultError(result);
            
        }
        else
        {
            return BadRequest("User already exists");
        }
    }

    [HttpPut("UpdateAgencyUser/{id}")]
    public async Task<IActionResult> UpdateAgencyUser([FromRoute] Guid id, [FromBody] IdModel agency)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }
        var claims = await _userManager.GetClaimsAsync(user);
        if (claims.Any(a => a.Value.Equals(agency.Id.ToString(), StringComparison.InvariantCultureIgnoreCase)))
        {
            ModelState.AddModelError(nameof(agency.Id), "The user already exists in this agency");
            return BadRequest(ModelState);
        }
        var result = await _userManager.AddClaimAsync(user, Constants.ClaimAgencyId(agency.Id));
        if (!result.Succeeded) return ReturnIdentityResultError(result);
        return Ok();
    }

    [HttpPut("UpdateEmail")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        CovenantUser user = await _userManager.FindByIdAsync(model.Id.ToString());
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "User not found");
            return BadRequest(ModelState);
        }

        user.Email = model.NewEmail;
        user.UserName = model.NewEmail;

        IdentityResult result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) return Ok();
        foreach (IdentityError error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return BadRequest(ModelState);
    }

    [HttpPut("DeleteUserOrClaim/{userId}")]
    public async Task<IActionResult> DeleteUserOrClaim([FromRoute] Guid userId, [FromBody] IdModel claim)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return NotFound();
        var claims = await _userManager.GetClaimsAsync(user);
        var userDeleted = false;
        if (claims.Count > 1)
        {
            var claimToDelete = claims.FirstOrDefault(c => c.Value == claim.Id.ToString());
            await _userManager.RemoveClaimAsync(user, claimToDelete);
        }
        else
        {
            await _userManager.DeleteAsync(user);
            userDeleted = true;
        }
        return Ok(userDeleted);
    }

    [HttpPost("CreateInactiveUser")]
    public async Task<IActionResult> CreateInactiveUser([FromServices] CovenantContext context, [FromBody] IdModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id.ToString());
        if (user == null)
        {
            return NotFound();
        }
        await context.InactiveUsers.AddAsync(new InactiveUser { UserId = user.Id });
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("UpdateRole")]
    public async Task<IActionResult> UpdateRole([FromServices] CovenantContext context, [FromBody] UpdateRoleModel model)
    {
        var userRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == model.Id);
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == model.Role);
        if (role is null)
        {
            ModelState.AddModelError(nameof(model.Role), $"Unknown role '{model.Role}'");
            return BadRequest(ModelState);
        }
        if (userRole != null)
        {
            userRole.RoleId = role.Id;
        }
        else
        {
            userRole = new IdentityUserRole<Guid> { UserId = model.Id, RoleId = role.Id };
            await context.UserRoles.AddAsync(userRole);
        }
        await context.SaveChangesAsync();
        return Ok();
    }

    private IActionResult ReturnIdentityResultError(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors) ModelState.AddModelError(error.Code, error.Description);
        return BadRequest(ModelState);
    }

    private async Task SendEmailToConfirmAndSetPassword(CovenantUser user)
    {
        try
        {
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            string action = Url.Action("CreatePassword", "Account", new { token = resetToken, id = user.Id });
            string link = string.Concat(configuration.GetAppUrl(), action);
            string html = await renderer.RenderViewToStringAsync("/Views/Notifications/ConfirmAccount.cshtml", new ConfirmAccountViewModel
            {
                Url = link,
                Message = Resources.Resources.ConfirmAndSetPassword
            });
            bool sendEmail = await emailService.SendEmail(user.Email, Resources.Resources.ConfirmYourAccount, html);
            if (!sendEmail) _logger.LogError("Error sending email confirmation link {Link}", link);
        }
        catch (Exception e)
        {
            _logger.LogError("Error sending confirmation email: {Error}", e);
        }
    }

    private async Task SendEmailToConfirm(CovenantUser user)
    {
        try
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string action = Url.Action("ConfirmEmailAddress", "Account", new { token, id = user.Id });
            string link = string.Concat(configuration.GetAppUrl(), action);
            string html = await renderer.RenderViewToStringAsync("/Views/Notifications/ConfirmAccount.cshtml", new ConfirmAccountViewModel
            {
                Url = link,
                Message = Resources.Resources.ConfirmAccountWorker
            });
            bool sendEmail = await emailService.SendEmail(user.Email, Resources.Resources.ConfirmYourAccount, html);
            if (!sendEmail) _logger.LogError("Error sending email confirmation link {Link}", link);
        }
        catch (Exception e)
        {
            _logger.LogError("Error sending confirmation email: {Error}", e);
        }
    }
}