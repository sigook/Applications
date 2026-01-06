using Covenant.Common.Entities;

namespace Covenant.Common.Repositories
{
    public interface IUserRepository
    {
        Task Create<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T: class;
        Task<User> GetUserById(Guid userId);
        Task<bool> UserExists(string email);
        Task<bool> UserIsWorker(Guid userId);
        Task<bool> UserIsCompany(Guid userId);
        Task<string> GetUserEmail(Guid userId);
        Task<User> GetUserByEmail(string email);
        Task SaveChangesAsync();
    }
}