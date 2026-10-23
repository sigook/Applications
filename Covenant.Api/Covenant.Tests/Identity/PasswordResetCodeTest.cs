using Covenant.Common.Entities.Identity;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Covenant.Common.Repositories.Identity;
using Covenant.Core.BL.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Covenant.Tests.Identity;

public class PasswordResetCodeTest
{
    private const string Email = "worker@sigook.com";
    private const string Code = "123456";
    private const string NewPassword = "NewSecret1";

    private readonly CovenantUser _user = new() { Id = Guid.NewGuid(), Email = Email };
    private readonly Mock<UserManager<CovenantUser>> _userManager = new(Mock.Of<IUserStore<CovenantUser>>(), null, null, null, null, null, null, null, null);
    private readonly Mock<IIdentityRepository> _repository = new();
    private readonly Mock<IAccountNotificationService> _notifications = new();
    private readonly PasswordHasher<CovenantUser> _hasher = new();
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 10, 23, 12, 0, 0, TimeSpan.Zero));
    private readonly PasswordResetService _sut;

    public PasswordResetCodeTest()
    {
        _userManager.Setup(u => u.FindByEmailAsync(Email)).ReturnsAsync(_user);
        _userManager.Setup(u => u.GeneratePasswordResetTokenAsync(_user)).ReturnsAsync("token");
        _userManager.Setup(u => u.ResetPasswordAsync(_user, "token", NewPassword)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(u => u.UpdateAsync(_user)).ReturnsAsync(IdentityResult.Success);
        _repository.Setup(r => r.GetPendingResetCodes(_user.Id)).ReturnsAsync([]);
        _sut = new PasswordResetService(_userManager.Object, _repository.Object, _hasher, _notifications.Object, _clock, NullLogger<PasswordResetService>.Instance);
    }

    private PasswordResetCode PendingCode(string code = Code, int minutesLeft = 10, int failedAttempts = 0)
    {
        var resetCode = new PasswordResetCode(_user.Id, _hasher.HashPassword(_user, code), _clock.GetUtcNow().AddMinutes(-5), _clock.GetUtcNow().AddMinutes(minutesLeft));
        for (var i = 0; i < failedAttempts; i++) resetCode.RegisterFailedAttempt();
        _repository.Setup(r => r.GetLatestPendingResetCode(_user.Id)).ReturnsAsync(resetCode);
        return resetCode;
    }

    [Fact]
    public async Task ResetPassword_ValidCode_ChangesThePasswordAndUnlocksTheAccount()
    {
        PasswordResetCode resetCode = PendingCode();

        PasswordResetResult result = await _sut.ResetPassword(Email, Code, NewPassword);

        Assert.True(result.Succeeded);
        Assert.True(resetCode.Consumed);
        Assert.True(_user.EmailConfirmed);
        _userManager.Verify(u => u.SetLockoutEndDateAsync(_user, null), Times.Once);
        _userManager.Verify(u => u.ResetAccessFailedCountAsync(_user), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_UnknownEmail_ReturnsInvalidCode()
    {
        PasswordResetResult result = await _sut.ResetPassword("nobody@sigook.com", Code, NewPassword);

        Assert.Equal(PasswordResetService.InvalidCode, result.Error);
    }

    [Fact]
    public async Task ResetPassword_WithoutPendingCode_ReturnsInvalidCode()
    {
        PasswordResetResult result = await _sut.ResetPassword(Email, Code, NewPassword);

        Assert.Equal(PasswordResetService.InvalidCode, result.Error);
    }

    [Fact]
    public async Task ResetPassword_ExpiredCode_ReturnsCodeExpired()
    {
        PendingCode(minutesLeft: -1);

        PasswordResetResult result = await _sut.ResetPassword(Email, Code, NewPassword);

        Assert.Equal(PasswordResetService.CodeExpired, result.Error);
        _userManager.Verify(u => u.ResetPasswordAsync(It.IsAny<CovenantUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RequestedCode_ExpiresAfterFifteenMinutes()
    {
        PasswordResetCode stored = null;
        string sent = null;
        _repository.Setup(r => r.Create(It.IsAny<PasswordResetCode>())).Callback<PasswordResetCode>(c => stored = c).Returns(Task.CompletedTask);
        _notifications.Setup(n => n.SendPasswordResetCode(_user, It.IsAny<string>(), It.IsAny<int>())).Callback<CovenantUser, string, int>((_, code, _) => sent = code).Returns(Task.CompletedTask);
        DateTimeOffset requestedAt = _clock.GetUtcNow();
        await _sut.RequestCode(Email);
        _repository.Setup(r => r.GetLatestPendingResetCode(_user.Id)).ReturnsAsync(stored);

        _clock.Advance(TimeSpan.FromMinutes(15) + TimeSpan.FromSeconds(1));
        PasswordResetResult result = await _sut.ResetPassword(Email, sent, NewPassword);

        Assert.Equal(requestedAt, stored.CreatedAt);
        Assert.Equal(requestedAt.AddMinutes(15), stored.ExpiresAt);
        Assert.Equal(PasswordResetService.CodeExpired, result.Error);
    }

    [Fact]
    public async Task RequestCode_AfterTheCooldown_SendsANewCode()
    {
        var recent = new PasswordResetCode(_user.Id, "hash", _clock.GetUtcNow(), _clock.GetUtcNow().AddMinutes(15));
        _repository.Setup(r => r.GetPendingResetCodes(_user.Id)).ReturnsAsync([recent]);

        await _sut.RequestCode(Email);
        _clock.Advance(TimeSpan.FromSeconds(61));
        await _sut.RequestCode(Email);

        _repository.Verify(r => r.Create(It.IsAny<PasswordResetCode>()), Times.Once);
        Assert.True(recent.Consumed);
    }

    [Fact]
    public async Task ResetPassword_WrongCode_CountsTheAttempt()
    {
        PasswordResetCode resetCode = PendingCode();

        PasswordResetResult result = await _sut.ResetPassword(Email, "999999", NewPassword);

        Assert.Equal(PasswordResetService.InvalidCode, result.Error);
        Assert.Equal(1, resetCode.Attempts);
        Assert.False(resetCode.Consumed);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_AfterFiveFailedAttempts_BurnsTheCodeEvenIfItIsCorrect()
    {
        PasswordResetCode resetCode = PendingCode(failedAttempts: 5);

        PasswordResetResult result = await _sut.ResetPassword(Email, Code, NewPassword);

        Assert.Equal(PasswordResetService.TooManyAttempts, result.Error);
        Assert.True(resetCode.Consumed);
        _userManager.Verify(u => u.ResetPasswordAsync(It.IsAny<CovenantUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResetPassword_PolicyRejection_KeepsTheCodeUsable()
    {
        PasswordResetCode resetCode = PendingCode();
        _userManager.Setup(u => u.ResetPasswordAsync(_user, "token", "weak"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "PasswordRequiresUpper", Description = "Passwords must have at least one uppercase ('A'-'Z')." }));

        PasswordResetResult result = await _sut.ResetPassword(Email, Code, "weak");

        Assert.Equal(PasswordResetService.PasswordPolicy, result.Error);
        Assert.Equal(["Passwords must have at least one uppercase ('A'-'Z')."], result.Messages);
        Assert.False(resetCode.Consumed);
    }

    [Fact]
    public async Task RequestCode_UnknownOrInactiveUser_SendsNothing()
    {
        _repository.Setup(r => r.IsInactive(_user.Id)).ReturnsAsync(true);

        await _sut.RequestCode("nobody@sigook.com");
        await _sut.RequestCode(Email);

        _repository.Verify(r => r.Create(It.IsAny<PasswordResetCode>()), Times.Never);
        _notifications.Verify(n => n.SendPasswordResetCode(It.IsAny<CovenantUser>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RequestCode_WithinTheResendCooldown_SendsNothing()
    {
        var recent = new PasswordResetCode(_user.Id, "hash", _clock.GetUtcNow().AddSeconds(-10), _clock.GetUtcNow().AddMinutes(15));
        _repository.Setup(r => r.GetPendingResetCodes(_user.Id)).ReturnsAsync([recent]);

        await _sut.RequestCode(Email);

        _repository.Verify(r => r.Create(It.IsAny<PasswordResetCode>()), Times.Never);
    }

    [Fact]
    public async Task RequestCode_SendsASixDigitCode_StoresOnlyItsHash_AndRetiresPreviousCodes()
    {
        var previous = new PasswordResetCode(_user.Id, "old-hash", _clock.GetUtcNow().AddMinutes(-10), _clock.GetUtcNow().AddMinutes(5));
        _repository.Setup(r => r.GetPendingResetCodes(_user.Id)).ReturnsAsync([previous]);
        PasswordResetCode stored = null;
        string sent = null;
        _repository.Setup(r => r.Create(It.IsAny<PasswordResetCode>())).Callback<PasswordResetCode>(c => stored = c).Returns(Task.CompletedTask);
        _notifications.Setup(n => n.SendPasswordResetCode(_user, It.IsAny<string>(), 15)).Callback<CovenantUser, string, int>((_, code, _) => sent = code).Returns(Task.CompletedTask);

        await _sut.RequestCode(Email);

        Assert.True(previous.Consumed);
        Assert.Matches("^[0-9]{6}$", sent);
        Assert.NotEqual(sent, stored.CodeHash);
        Assert.Equal(PasswordVerificationResult.Success, _hasher.VerifyHashedPassword(_user, stored.CodeHash, sent));
        Assert.True(stored.ExpiresAt > _clock.GetUtcNow().AddMinutes(14));
    }
}
