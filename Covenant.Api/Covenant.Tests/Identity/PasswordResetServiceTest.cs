using Covenant.Common.Entities;
using Covenant.Common.Entities.Identity;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Repositories.Identity;
using Covenant.Core.BL.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Covenant.Tests.Identity;

public class PasswordResetServiceTest
{
    private const string Email = "worker@sigook.com";

    private readonly CovenantUser _user = new() { Id = Guid.NewGuid(), Email = Email };
    private readonly Mock<UserManager<CovenantUser>> _userManager = new(Mock.Of<IUserStore<CovenantUser>>(), null, null, null, null, null, null, null, null);
    private readonly Mock<IIdentityRepository> _repository = new();
    private readonly Mock<IAccountNotificationService> _notifications = new();
    private readonly PasswordResetService _sut;

    public PasswordResetServiceTest()
    {
        _userManager.Setup(um => um.FindByEmailAsync(Email)).ReturnsAsync(_user);
        _repository.Setup(r => r.GetPendingResetCodes(_user.Id)).ReturnsAsync([]);
        _sut = new PasswordResetService(
            _userManager.Object,
            _repository.Object,
            new PasswordHasher<CovenantUser>(),
            _notifications.Object,
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<PasswordResetService>.Instance);
    }

    [Fact]
    public async Task Sends_A_Code_While_The_User_Is_Under_Both_Limits()
    {
        GivenCodesInTheLastHour(PasswordResetService.MaxCodesPerHour - 1);
        GivenCodesInTheLastDay(PasswordResetService.MaxCodesPerDay - 1);

        await _sut.RequestCode(Email);

        _repository.Verify(r => r.AddResetCode(It.Is<PasswordResetCode>(prc => prc.UserId == _user.Id)), Times.Once);
        _notifications.Verify(n => n.SendPasswordResetCode(_user, It.IsAny<string>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Does_Not_Send_A_Code_Once_The_Hourly_Limit_Is_Reached()
    {
        GivenCodesInTheLastHour(PasswordResetService.MaxCodesPerHour);
        GivenCodesInTheLastDay(PasswordResetService.MaxCodesPerHour);

        await _sut.RequestCode(Email);

        _repository.Verify(r => r.AddResetCode(It.IsAny<PasswordResetCode>()), Times.Never);
        _notifications.Verify(n => n.SendPasswordResetCode(It.IsAny<CovenantUser>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Does_Not_Send_A_Code_Once_The_Daily_Limit_Is_Reached()
    {
        GivenCodesInTheLastHour(0);
        GivenCodesInTheLastDay(PasswordResetService.MaxCodesPerDay);

        await _sut.RequestCode(Email);

        _repository.Verify(r => r.AddResetCode(It.IsAny<PasswordResetCode>()), Times.Never);
        _notifications.Verify(n => n.SendPasswordResetCode(It.IsAny<CovenantUser>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Sends_Links_Only_Up_To_The_Hourly_Limit()
    {
        for (var i = 0; i < PasswordResetService.MaxLinksPerHour + 2; i++)
        {
            await _sut.RequestLink(Email);
        }

        _notifications.Verify(n => n.SendPasswordResetLink(_user), Times.Exactly(PasswordResetService.MaxLinksPerHour));
    }

    [Fact]
    public async Task Does_Not_Send_A_Link_For_An_Unknown_Email()
    {
        await _sut.RequestLink("unknown@sigook.com");

        _notifications.Verify(n => n.SendPasswordResetLink(It.IsAny<CovenantUser>()), Times.Never);
    }

    private void GivenCodesInTheLastHour(int count) =>
        _repository.Setup(r => r.CountResetCodesSince(_user.Id, It.Is<DateTimeOffset>(since => since > DateTimeOffset.UtcNow.AddHours(-2))))
            .ReturnsAsync(count);

    private void GivenCodesInTheLastDay(int count) =>
        _repository.Setup(r => r.CountResetCodesSince(_user.Id, It.Is<DateTimeOffset>(since => since <= DateTimeOffset.UtcNow.AddHours(-2))))
            .ReturnsAsync(count);
}
