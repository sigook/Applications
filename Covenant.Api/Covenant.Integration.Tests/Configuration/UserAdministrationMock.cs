using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Identity;
using Moq;

namespace Covenant.Integration.Tests.Configuration;

public static class UserAdministrationMock
{
    public static Mock<IUserAdministrationService> Create(Guid? createdUserId = null, bool userDeleted = true, params UserRoleModel[] roles)
    {
        var mock = new Mock<IUserAdministrationService>();
        mock.Setup(m => m.CreateUser(It.IsAny<CreateUserModel>())).ReturnsAsync((CreateUserModel model) =>
        {
            var userId = createdUserId ?? Guid.NewGuid();
            if (model.AgencyId.HasValue) TestAgencyMembers.Add(model.AgencyId.Value, userId);
            return Result.Ok(userId);
        });
        mock.Setup(m => m.UpdateAgencyClaims(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync((Guid userId, IEnumerable<Guid> add, IEnumerable<Guid> remove) =>
            {
                foreach (var agencyId in remove) TestAgencyMembers.Remove(agencyId, userId);
                foreach (var agencyId in add) TestAgencyMembers.Add(agencyId, userId);
                return Result.Ok();
            });
        mock.Setup(m => m.RemoveClaimOrDeleteUser(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((Guid userId, Guid claimValue) =>
            {
                TestAgencyMembers.Remove(claimValue, userId);
                return Result.Ok(userDeleted);
            });
        mock.Setup(m => m.Deactivate(It.IsAny<Guid>())).ReturnsAsync(Result.Ok());
        mock.Setup(m => m.UpdateEmail(It.IsAny<UpdateEmailModel>())).ReturnsAsync(Result.Ok());
        mock.Setup(m => m.UpdateRole(It.IsAny<UpdateRoleModel>())).ReturnsAsync(Result.Ok());
        mock.Setup(m => m.GetUsersRoles(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(roles);
        mock.Setup(m => m.GetAgencyMembers(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync((IEnumerable<Guid> agencyIds) => TestAgencyMembers.OfAgencies(agencyIds));
        mock.Setup(m => m.GetUsersAgencies(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync((IEnumerable<Guid> userIds) => TestAgencyMembers.OfUsers(userIds));
        return mock;
    }
}
