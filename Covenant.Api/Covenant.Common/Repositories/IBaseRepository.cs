namespace Covenant.Common.Repositories;

public interface IBaseRepository
{
    Task Create<T>(T entity) where T : class;
    Task CreateRange<T>(IEnumerable<T> entities) where T : class;
    void Update<T>(T entity) where T : class;
    void UpdateRange<T>(IEnumerable<T> entities) where T : class;
    void Delete<T>(T entity) where T : class;
    void DeleteRange<T>(IEnumerable<T> entities) where T : class;
    Task SaveChangesAsync();
}
