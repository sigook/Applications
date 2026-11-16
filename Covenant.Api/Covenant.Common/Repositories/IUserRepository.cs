using Covenant.Common.Entities;
using Covenant.Common.Models.Identity;

namespace Covenant.Common.Repositories;

public interface IUserRepository : IBaseRepository
{
    Task<User> GetUserById(Guid userId);
    Task<bool> UserExists(string email);
    Task<bool> UserIsWorker(Guid userId);
    Task<bool> UserIsCompany(Guid userId);
    Task<string> GetUserEmail(Guid userId);
    Task<AccountModel> GetAccount(Guid userId);
    Task<User> GetUserByEmail(string email);
}
