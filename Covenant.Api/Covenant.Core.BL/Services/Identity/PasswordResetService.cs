using Covenant.Common.Entities;
using Covenant.Common.Entities.Identity;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Covenant.Common.Repositories.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Covenant.Core.BL.Services.Identity;

public class PasswordResetService(
    UserManager<CovenantUser> userManager,
    IIdentityRepository identityRepository,
    IPasswordHasher<CovenantUser> passwordHasher,
    IAccountNotificationService notifications,
    ILogger<PasswordResetService> logger) : IPasswordResetService
{
    public const string InvalidCode = "invalid_code";
    public const string CodeExpired = "code_expired";
    public const string TooManyAttempts = "too_many_attempts";
    public const string PasswordPolicy = "password_policy";

    private const int CodeLifetimeMinutes = 15;
    private const int MaxAttempts = 5;
    private const int ResendCooldownSeconds = 60;

    public async Task RequestCode(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            logger.LogInformation("Password reset code requested for unknown email");
            return;
        }

        if (await identityRepository.IsInactive(user.Id))
        {
            logger.LogWarning("Password reset code requested for inactive user. UserId={UserId}", user.Id);
            return;
        }

        var previousCodes = await identityRepository.GetPendingResetCodes(user.Id);
        var now = DateTimeOffset.UtcNow;
        var recentCode = previousCodes.FirstOrDefault(prc => prc.CreatedAt > now.AddSeconds(-ResendCooldownSeconds));
        if (recentCode is not null)
        {
            logger.LogInformation("Password reset code request throttled. UserId={UserId} LastCodeAt={LastCodeAt} Now={Now}",
                user.Id, recentCode.CreatedAt, now);
            return;
        }

        previousCodes.ForEach(prc => prc.Consumed = true);

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        await identityRepository.AddResetCode(new PasswordResetCode
        {
            UserId = user.Id,
            CodeHash = passwordHasher.HashPassword(user, code),
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(CodeLifetimeMinutes)
        });
        await identityRepository.SaveChangesAsync();

        await notifications.SendPasswordResetCode(user, code, CodeLifetimeMinutes);
    }

    public async Task<PasswordResetResult> ResetPassword(string email, string code, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return PasswordResetResult.Fail(InvalidCode);

        var resetCode = await identityRepository.GetLatestPendingResetCode(user.Id);
        if (resetCode is null) return PasswordResetResult.Fail(InvalidCode);
        if (resetCode.ExpiresAt < DateTimeOffset.UtcNow) return PasswordResetResult.Fail(CodeExpired);

        if (resetCode.Attempts >= MaxAttempts)
        {
            resetCode.Consumed = true;
            await identityRepository.SaveChangesAsync();
            return PasswordResetResult.Fail(TooManyAttempts);
        }

        if (passwordHasher.VerifyHashedPassword(user, resetCode.CodeHash, code) == PasswordVerificationResult.Failed)
        {
            resetCode.Attempts++;
            await identityRepository.SaveChangesAsync();
            logger.LogWarning("Invalid password reset code attempt {Attempts}/{MaxAttempts}. UserId={UserId}",
                resetCode.Attempts, MaxAttempts, user.Id);
            return PasswordResetResult.Fail(InvalidCode);
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            return PasswordResetResult.Fail(PasswordPolicy, result.Errors.Select(e => e.Description).ToList());

        resetCode.Consumed = true;
        await identityRepository.SaveChangesAsync();

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            var update = await userManager.UpdateAsync(user);
            if (!update.Succeeded) logger.LogError("Confirm account failed. UserId={UserId}", user.Id);
        }

        await userManager.SetLockoutEndDateAsync(user, null);
        await userManager.ResetAccessFailedCountAsync(user);

        logger.LogInformation("Password reset via code succeeded. UserId={UserId}", user.Id);
        return PasswordResetResult.Success();
    }
}
