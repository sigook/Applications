using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models;
using Covenant.Common.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Services.Identity;

public class AccountNotificationService(
    UserManager<CovenantUser> userManager,
    IRazorViewToStringRenderer renderer,
    IEmailService emailService,
    IOptions<IdentityConfiguration> options,
    ILogger<AccountNotificationService> logger) : IAccountNotificationService
{
    private const string ConfirmAccountView = "/Views/Notifications/Identity/ConfirmAccount.cshtml";
    private const string ResetPasswordView = "/Views/Notifications/Identity/ResetPassword.cshtml";
    private const string PasswordResetCodeView = "/Views/Notifications/Identity/PasswordResetCode.cshtml";

    public async Task SendConfirmAccount(CovenantUser user, string message)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = AccountLink("ConfirmEmailAddress", token, user.Id);
        await Send(user.Email, AccountMessages.ConfirmYourAccount, ConfirmAccountView, new ConfirmAccountViewModel { Url = url, Message = message });
    }

    public async Task SendConfirmAndSetPassword(CovenantUser user)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var url = AccountLink("CreatePassword", token, user.Id);
        await Send(user.Email, AccountMessages.ConfirmYourAccount, ConfirmAccountView, new ConfirmAccountViewModel { Url = url, Message = AccountMessages.ConfirmAndSetPassword });
    }

    public async Task SendPasswordResetLink(CovenantUser user)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var url = AccountLink("ResetPassword", token, user.Id);
        await Send(user.Email, AccountMessages.ResetPassword, ResetPasswordView, new ResetPasswordViewModel { Url = url });
    }

    public Task SendPasswordResetCode(CovenantUser user, string code, int expiresMinutes) =>
        Send(user.Email, AccountMessages.PasswordResetCode, PasswordResetCodeView, new PasswordResetCodeViewModel { Code = code, ExpiresMinutes = expiresMinutes });

    private string AccountLink(string action, string token, Guid userId) =>
        $"{options.Value.IssuerUri?.TrimEnd('/')}/Account/{action}?token={Uri.EscapeDataString(token)}&id={userId}";

    private async Task Send<TModel>(string email, string subject, string view, TModel model)
    {
        try
        {
            var html = await renderer.RenderViewToStringAsync(view, model);
            var sent = await emailService.SendEmail(new EmailParams(email, subject, html)
            {
                EmailSettingName = EmailSettingName.SigookNotification
            });
            if (!sent) logger.LogError("Error sending {Subject} email to {Email}", subject, email);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending {Subject} email to {Email}", subject, email);
        }
    }
}
