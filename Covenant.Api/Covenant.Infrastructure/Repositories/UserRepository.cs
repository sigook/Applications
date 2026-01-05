using Covenant.Common.Entities;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CovenantContext _context;

        public UserRepository(CovenantContext context) => _context = context;

        public async Task Create<T>(T entity) where T: class  => await _context.Set<T>().AddAsync(entity);

        public void Update<T>(T entity) where T: class => _context.Set<T>().Update(entity);

        public void Delete<T>(T entity) where T: class => _context.Set<T>().Remove(entity);

        public Task<User> GetUserById(Guid userId) => _context.User.FirstOrDefaultAsync(c => c.Id == userId);

        public Task<string> GetUserEmail(Guid userId) => _context.User.Where(c => c.Id == userId).Select(c => c.Email).AsNoTracking().SingleOrDefaultAsync();

        public Task<User> GetUserByEmail(string email) => _context.User.SingleOrDefaultAsync(c => c.Email.ToLower().Equals(email.ToLower()));

        public async Task<bool> UserExists(string email) => await _context.User.AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        public Task<bool> UserIsWorker(Guid userId) => _context.WorkerProfile.AnyAsync(c => c.WorkerId == userId);

        public Task<bool> UserIsCompany(Guid userId) => _context.CompanyProfile.AnyAsync(c => c.CompanyId == userId);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}