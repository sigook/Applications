namespace Covenant.Common.Repositories
{
    public interface IBaseRepository<in T> where T : class
    {
        Task Create(IEnumerable<T> entities);
        Task Create(T entity);
        Task Update(params T[] entity);
        Task SaveChangesAsync();
    }
}