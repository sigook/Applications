using Covenant.Common.Functionals;
using Covenant.Common.Models.Security;

namespace Covenant.Common.Interfaces.Identity;

public interface IUserAdministrationService
{
    Task<Result<Guid>> CreateUser(CreateUserModel model);
    Task<Result> AddAgencyClaim(Guid userId, Guid agencyId);
    Task<Result<bool>> RemoveClaimOrDeleteUser(Guid userId, Guid claimValue);
    Task<Result> Deactivate(Guid userId);
    Task<Result> UpdateEmail(UpdateEmailModel model);
    Task<Result> UpdateRole(UpdateRoleModel model);
    Task<IReadOnlyList<UserRoleModel>> GetUsersRoles(IEnumerable<Guid> userIds);
}
