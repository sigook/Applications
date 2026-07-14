namespace Covenant.Common.Repositories.Company;

public interface ICompanyInteractionRepository
{
    Task Create<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
}