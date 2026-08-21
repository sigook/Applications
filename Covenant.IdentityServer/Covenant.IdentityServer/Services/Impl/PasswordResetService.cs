using System.Security.Cryptography;
using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Entities;
using Covenant.IdentityServer.Models;
using Covenant.IdentityServer.Views.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.Services.Impl;

public class PasswordResetService : IPasswordResetService
{
    public const string InvalidCode = "invalid_code";
    public const string CodeExpired = "code_expired";
    public const string TooManyAttempts = "too_many_attempts";
    public const string PasswordPolicy = "password_policy";

    private const int CodeLifetimeMinutes = 15;
    private const int MaxAttempts = 5;
    private const int ResendCooldownSeconds = 60;

    private readonly UserManager<CovenantUser> _userManager;
    private readonly CovenantContext _context;
    private readonly IPasswordHasher<CovenantUser> _passwordHasher;
    private readonly IRazorViewToStringRenderer _renderer;
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        UserManager<CovenantUser> userManager,
        CovenantContext context,
        IPasswordHasher<CovenantUser> passwordHasher,
        IRazorViewToStringRenderer renderer,
        IEmailService emailService,
        ILogger<PasswordResetService> logger)
    {
        _userManager = userManager;
        _context = context;
        _passwordHasher = passwordHasher;
        _renderer = renderer;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task RequestCodeAsync(string email)
    {
        CovenantUser user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogInformation("Password reset code requested for unknown email");
            return;
        }

        bool isInactive = await _context.InactiveUsers.AnyAsync(iu => iu.UserId == user.Id);
        if (isInactive)
        {
            _logger.LogWarning("Password reset code requested for inactive user. UserId={UserId}", user.Id);
            return;
        }

        List<PasswordResetCode> previousCodes = await _context.PasswordResetCodes
            .Where(prc => prc.UserId == user.Id && !prc.Consumed)
            .ToListAsync();

        DateTimeOffset now = DateTimeOffset.UtcNow;
        PasswordResetCode recentCode = previousCodes
            .FirstOrDefault(prc => prc.CreatedAt > now.AddSeconds(-ResendCooldownSeconds));
        if (recentCode is not null)
        {
            _logger.LogInformation("Password reset code request throttled. UserId={UserId} LastCodeAt={LastCodeAt} Now={Now}",
                user.Id, recentCode.CreatedAt, now);
            return;
        }

        previousCodes.ForEach(prc => prc.Consumed = true);

        string code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        await _context.PasswordResetCodes.AddAsync(new PasswordResetCode
        {
            UserId = user.Id,
            CodeHash = _passwordHasher.HashPassword(user, code),
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(CodeLifetimeMinutes)
        });
        await _context.SaveChangesAsync();

        string html = await _renderer.RenderViewToStringAsync("/Views/Notifications/PasswordResetCode.cshtml",
            new PasswordResetCodeViewModel { Code = code, ExpiresMinutes = CodeLifetimeMinutes });
        bool sent = await _emailService.SendEmail(user.Email, Resources.Resources.PasswordResetCode, html);
        if (!sent) _logger.LogError("Error sending password reset code email. UserId={UserId}", user.Id);
    }

    public async Task<PasswordResetResult> ResetPasswordAsync(string email, string code, string newPassword)
    {
        CovenantUser user = await _userManager.FindByEmailAsync(email);
        if (user is null) return PasswordResetResult.Fail(InvalidCode);

        PasswordResetCode resetCode = await _context.PasswordResetCodes
            .Where(prc => prc.UserId == user.Id && !prc.Consumed)
            .OrderByDescending(prc => prc.CreatedAt)
            .FirstOrDefaultAsync();
        if (resetCode is null) return PasswordResetResult.Fail(InvalidCode);
        if (resetCode.ExpiresAt < DateTimeOffset.UtcNow) return PasswordResetResult.Fail(CodeExpired);

        if (resetCode.Attempts >= MaxAttempts)
        {
            resetCode.Consumed = true;
            await _context.SaveChangesAsync();
            return PasswordResetResult.Fail(TooManyAttempts);
        }

        if (_passwordHasher.VerifyHashedPassword(user, resetCode.CodeHash, code) == PasswordVerificationResult.Failed)
        {
            resetCode.Attempts++;
            await _context.SaveChangesAsync();
            _logger.LogWarning("Invalid password reset code attempt {Attempts}/{MaxAttempts}. UserId={UserId}",
                resetCode.Attempts, MaxAttempts, user.Id);
            return PasswordResetResult.Fail(InvalidCode);
        }

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            return PasswordResetResult.Fail(PasswordPolicy, result.Errors.Select(e => e.Description).ToList());

        resetCode.Consumed = true;
        await _context.SaveChangesAsync();

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            IdentityResult update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded) _logger.LogError("Confirm account failed. UserId={UserId}", user.Id);
        }

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);

        _logger.LogInformation("Password reset via code succeeded. UserId={UserId}", user.Id);
        return PasswordResetResult.Success();
    }
}
