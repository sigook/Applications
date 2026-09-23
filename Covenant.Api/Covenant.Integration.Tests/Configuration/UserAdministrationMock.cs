using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Security;
using Moq;

namespace Covenant.Integration.Tests.Configuration;

public static class UserAdministrationMock
{
    public static Mock<IUserAdministrationService> Create(Guid? createdUserId = null, bool userDeleted = true, params UserRoleModel[] roles)
    {
        var mock = new Mock<IUserAdministrationService>();
        mock.Setup(m => m.CreateUser(It.IsAny<CreateUserModel>())).ReturnsAsync(() => Result.Ok(createdUserId ?? Guid.NewGuid()));
        mock.Setup(m => m.AddAgencyClaim(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Ok());
        mock.Setup(m => m.RemoveClaimOrDeleteUser(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(Result.Ok(userDeleted));
        mock.Setup(m => m.Deactivate(It.IsAny<Guid>())).ReturnsAsync(Result.Ok());
        mock.Setup(m => m.UpdateEmail(It.IsAny<UpdateEmailModel>())).ReturnsAsync(Result.Ok());
        mock.Setup(m => m.UpdateRole(It.IsAny<UpdateRoleModel>())).ReturnsAsync(Result.Ok());
        mock.Setup(m => m.GetUsersRoles(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(roles);
        mock.Setup(m => m.HashPassword(It.IsAny<string>())).Returns<string>(password => $"hashed:{password}");
        return mock;
    }
}
