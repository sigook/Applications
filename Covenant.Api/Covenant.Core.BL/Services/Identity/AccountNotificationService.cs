using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models;
using Covenant.Common.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Covenant.Core.BL.Services.Identity;

public class AccountNotificationService(
    UserManager<CovenantUser> userManager,
    IRazorViewToStringRenderer renderer,
    IEmailService emailService,
    IConfiguration configuration,
    ILogger<AccountNotificationService> logger) : IAccountNotificationService
{
    private const string ConfirmYourAccount = "Confirm your account";
    private const string ConfirmAccountMessage = "Welcome to Sigook. Please confirm your account by clicking bellow.";
    private const string ConfirmAccountView = "/Views/Notifications/Identity/ConfirmAccount.cshtml";
    private const string PasswordResetCodeView = "/Views/Notifications/Identity/PasswordResetCode.cshtml";

    public async Task SendConfirmAccount(CovenantUser user, string message)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = WebLink("confirm-email", token, user.Id);
        await Send(user.Email, ConfirmYourAccount, ConfirmAccountView, new ConfirmAccountViewModel { Url = url, Message = message });
    }

    public async Task SendConfirmAndSetPassword(CovenantUser user)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var url = WebLink("create-password", token, user.Id);
        await Send(user.Email, ConfirmYourAccount, ConfirmAccountView, new ConfirmAccountViewModel { Url = url, Message = "Welcome to Sigook. Please confirm your account and create your password by clicking bellow." });
    }

    public async Task ResendConfirmAccount(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || user.EmailConfirmed) return;
        await SendConfirmAccount(user, ConfirmAccountMessage);
    }

    public Task SendPasswordResetCode(CovenantUser user, string code, int expiresMinutes) =>
        Send(user.Email, "Your password reset code", PasswordResetCodeView, new PasswordResetCodeViewModel { Code = code, ExpiresMinutes = expiresMinutes });

    private string WebLink(string path, string token, Guid userId) =>
        $"{configuration["WebClientUrl"]?.TrimEnd('/')}/{path}?token={Uri.EscapeDataString(token)}&id={userId}";

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
