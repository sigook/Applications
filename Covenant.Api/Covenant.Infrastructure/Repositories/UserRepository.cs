using Covenant.Common.Entities;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories;

public class UserRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), IUserRepository
{
    public Task<User> GetUserById(Guid userId) => Context.Users.FirstOrDefaultAsync(c => c.Id == userId);

    public Task<string> GetUserEmail(Guid userId) => Context.Users.Where(c => c.Id == userId).Select(c => c.Email).AsNoTracking().SingleOrDefaultAsync();

    public Task<User> GetUserByEmail(string email) => Context.Users.SingleOrDefaultAsync(c => c.Email.ToLower().Equals(email.ToLower()));

    public async Task<bool> UserExists(string email) => await Context.Users.AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

    public Task<bool> UserIsWorker(Guid userId) => Context.WorkerProfiles.AnyAsync(c => c.WorkerId == userId);

    public Task<bool> UserIsCompany(Guid userId) => Context.CompanyProfiles.AnyAsync(c => c.CompanyId == userId);
}
