using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;

namespace Covenant.Common.Interfaces;

public interface IUserAccountService
{
    Task<Result<User>> CreateUser(CreateUserModel model);
    Task<Result> UpdateAgencyUser(Guid userId, IdModel agency);
    Task<Result> DeleteUserOrClaim(Guid userId, IdModel claim);
    Task<Result> InactiveUser(Guid id);
    Task<Result> UpdateUserEmail(UpdateEmailModel model);
    Task<Result> ChangeEmail(ChangeEmailModel model);
    Task<Result> UpdateUserRole(UpdateRoleModel model);
    Task<Result<IEnumerable<UserRoleModel>>> GetUsersRoles(IEnumerable<Guid> userIds);
}
