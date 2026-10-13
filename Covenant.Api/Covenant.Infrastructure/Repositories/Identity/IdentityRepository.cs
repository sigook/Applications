using Covenant.Common.Entities.Identity;
using Covenant.Common.Models.Identity;
using Covenant.Common.Repositories.Identity;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Covenant.Infrastructure.Repositories.Identity;

public class IdentityRepository(IdentityContext context, IOpenIddictTokenManager tokenManager) : BaseRepository<IdentityContext>(context), IIdentityRepository
{
    public Task<bool> IsInactive(Guid userId) => Context.InactiveUsers.AnyAsync(iu => iu.UserId == userId);

    public async Task RevokeTokens(Guid userId)
    {
        await foreach (var token in tokenManager.FindBySubjectAsync(userId.ToString()))
        {
            await tokenManager.TryRevokeAsync(token);
        }
    }

    public async Task<IReadOnlyList<UserRoleModel>> GetUsersRoles(IEnumerable<Guid> userIds) =>
        await Context.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(Context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new UserRoleModel(ur.UserId, r.Name))
            .ToListAsync();

    public Task<List<PasswordResetCode>> GetPendingResetCodes(Guid userId) =>
        Context.PasswordResetCodes.Where(prc => prc.UserId == userId && !prc.Consumed).ToListAsync();

    public Task<PasswordResetCode> GetLatestPendingResetCode(Guid userId) =>
        Context.PasswordResetCodes
            .Where(prc => prc.UserId == userId && !prc.Consumed)
            .OrderByDescending(prc => prc.CreatedAt)
            .FirstOrDefaultAsync();

    public Task<int> CountResetCodesSince(Guid userId, DateTimeOffset since) =>
        Context.PasswordResetCodes.CountAsync(prc => prc.UserId == userId && prc.CreatedAt > since);
}
