using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;

namespace Covenant.Common.Interfaces;

public interface IIdentityServerService
{
    Task<Result<User>> CreateUser(CreateUserModel model);
    Task<Result> UpdateAgencyUser(Guid userId, IdModel agency);
    Task<Result> DeleteUserOrClaim(Guid userId, IdModel claim);
    Result<string> HashPassword(string password);
    Task<Result> InactiveUser(Guid id);
    Task<Result> UpdateUserEmail(UpdateEmailModel model);
    Task<Result> UpdateUserRole(UpdateRoleModel model);
    Guid GetCompanyId();
    Guid GetAgencyId();
    string GetNickname();
    Guid GetUserId();
    bool IsAdmin();
    IEnumerable<Guid> GetAgencyIds();
}