using Covenant.Common.Entities.Identity;
using Covenant.Common.Models.Identity;

namespace Covenant.Common.Repositories.Identity;

public interface IIdentityRepository : IBaseRepository
{
    Task<bool> IsInactive(Guid userId);
    Task RevokeTokens(Guid userId);
    Task<IReadOnlyList<UserRoleModel>> GetUsersRoles(IEnumerable<Guid> userIds);
    Task<List<PasswordResetCode>> GetPendingResetCodes(Guid userId);
    Task<PasswordResetCode> GetLatestPendingResetCode(Guid userId);
    Task<int> CountResetCodesSince(Guid userId, DateTimeOffset since);
}
