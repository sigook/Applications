using Covenant.Common.Entities.Identity;
using Covenant.Common.Models.Security;

namespace Covenant.Common.Repositories.Identity;

public interface IIdentityRepository
{
    Task<bool> IsInactive(Guid userId);
    Task AddInactiveUser(Guid userId);
    Task RevokeTokens(Guid userId);
    Task<IReadOnlyList<UserRoleModel>> GetUsersRoles(IEnumerable<Guid> userIds);
    Task<List<PasswordResetCode>> GetPendingResetCodes(Guid userId);
    Task<PasswordResetCode> GetLatestPendingResetCode(Guid userId);
    Task AddResetCode(PasswordResetCode code);
    Task SaveChangesAsync();
}
