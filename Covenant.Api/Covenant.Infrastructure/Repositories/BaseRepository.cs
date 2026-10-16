using Covenant.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories;

public abstract class BaseRepository<TContext>(TContext context) : IBaseRepository where TContext : DbContext
{
    protected TContext Context { get; } = context;

    public async Task Create<T>(T entity) where T : class => await Context.Set<T>().AddAsync(entity);

    public Task CreateRange<T>(IEnumerable<T> entities) where T : class => Context.Set<T>().AddRangeAsync(entities);

    public void Update<T>(T entity) where T : class => Context.Set<T>().Update(entity);

    public void UpdateRange<T>(IEnumerable<T> entities) where T : class => Context.Set<T>().UpdateRange(entities);

    public void Delete<T>(T entity) where T : class => Context.Set<T>().Remove(entity);

    public void DeleteRange<T>(IEnumerable<T> entities) where T : class => Context.Set<T>().RemoveRange(entities);

    public Task SaveChangesAsync() => Context.SaveChangesAsync();
}
