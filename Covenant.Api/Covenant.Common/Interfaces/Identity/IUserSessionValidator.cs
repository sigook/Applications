using Covenant.Common.Entities;

namespace Covenant.Common.Interfaces.Identity;

public interface IUserSessionValidator
{
    Task<bool> IsActive(CovenantUser user);
}
