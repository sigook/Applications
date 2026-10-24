using Covenant.Common.Constants;
using Covenant.Common.Entities.Identity;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Repositories.Identity;
using Covenant.Core.BL.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Covenant.Tests.Identity;

public class UserSessionValidatorTest
{
    private readonly Mock<UserManager<CovenantUser>> _userManager = new(Mock.Of<IUserStore<CovenantUser>>(), null, null, null, null, null, null, null, null);
    private readonly Mock<IIdentityRepository> _repository = new();
    private readonly Mock<IMicrosoft365AccountService> _microsoft365 = new();
    private readonly UserSessionValidator _sut;
    private readonly CovenantUser _user = new() { Id = Guid.NewGuid(), Email = "staff@covenant.com" };

    public UserSessionValidatorTest()
    {
        _userManager.Setup(u => u.GetClaimsAsync(_user)).ReturnsAsync([]);
        _sut = new UserSessionValidator(_userManager.Object, _repository.Object, _microsoft365.Object, NullLogger<UserSessionValidator>.Instance);
    }

    private void LinkedToMicrosoft365(string objectId) =>
        _userManager.Setup(u => u.GetClaimsAsync(_user)).ReturnsAsync([IdentityClaims.MicrosoftObject(objectId), new Claim(CovenantConstants.AgencyId, Guid.NewGuid().ToString())]);

    [Fact]
    public async Task LocalActiveUser_IsActive()
    {
        Assert.True(await _sut.IsActive(_user));
        _microsoft365.Verify(m => m.IsAccountEnabled(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeactivatedUser_IsRejectedWithoutCallingMicrosoft365()
    {
        _repository.Setup(r => r.IsInactive(_user.Id)).ReturnsAsync(true);
        LinkedToMicrosoft365("oid-1");

        Assert.False(await _sut.IsActive(_user));
        _microsoft365.Verify(m => m.IsAccountEnabled(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Microsoft365User_FollowsTheAccountEnabledFlag(bool enabled)
    {
        LinkedToMicrosoft365("oid-1");
        _microsoft365.Setup(m => m.IsAccountEnabled("oid-1")).ReturnsAsync(enabled);

        Assert.Equal(enabled, await _sut.IsActive(_user));
    }
}
