using Covenant.Common.Repositories;
using Covenant.Infrastructure.Context;

namespace Covenant.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly CovenantContext _context;

        protected BaseRepository(CovenantContext context) => _context = context;

        public Task Create(IEnumerable<T> entities) => _context.AddRangeAsync(entities);

        public async Task Create(T entity) => await _context.AddAsync(entity);

        public Task Update(params T[] entity)
        {
            foreach (T e in entity) _context.Update(e);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}