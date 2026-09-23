using Covenant.Common.Entities.Identity;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Identity;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Covenant.Infrastructure.Repositories.Identity;

public class IdentityRepository(IdentityContext context, IOpenIddictTokenManager tokenManager) : IIdentityRepository
{
    public Task<bool> IsInactive(Guid userId) => context.InactiveUsers.AnyAsync(iu => iu.UserId == userId);

    public async Task AddInactiveUser(Guid userId) => await context.InactiveUsers.AddAsync(new InactiveUser { UserId = userId });

    public async Task RevokeTokens(Guid userId)
    {
        await foreach (var token in tokenManager.FindBySubjectAsync(userId.ToString()))
        {
            await tokenManager.TryRevokeAsync(token);
        }
    }

    public async Task<IReadOnlyList<UserRoleModel>> GetUsersRoles(IEnumerable<Guid> userIds) =>
        await context.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new UserRoleModel(ur.UserId, r.Name))
            .ToListAsync();

    public Task<List<PasswordResetCode>> GetPendingResetCodes(Guid userId) =>
        context.PasswordResetCodes.Where(prc => prc.UserId == userId && !prc.Consumed).ToListAsync();

    public Task<PasswordResetCode> GetLatestPendingResetCode(Guid userId) =>
        context.PasswordResetCodes
            .Where(prc => prc.UserId == userId && !prc.Consumed)
            .OrderByDescending(prc => prc.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task AddResetCode(PasswordResetCode code) => await context.PasswordResetCodes.AddAsync(code);

    public Task<int> CountResetCodesSince(Guid userId, DateTimeOffset since) =>
        context.PasswordResetCodes.CountAsync(prc => prc.UserId == userId && prc.CreatedAt > since);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
