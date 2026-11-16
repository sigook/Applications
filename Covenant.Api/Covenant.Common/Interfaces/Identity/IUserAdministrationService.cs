using Covenant.Common.Functionals;
using Covenant.Common.Models.Identity;

namespace Covenant.Common.Interfaces.Identity;

public interface IUserAdministrationService
{
    Task<Result<Guid>> CreateUser(CreateUserModel model);
    Task<Result> UpdateAgencyClaims(Guid userId, IEnumerable<Guid> add, IEnumerable<Guid> remove);
    Task<Result<bool>> RemoveClaimOrDeleteUser(Guid userId, Guid claimValue);
    Task<Result> Deactivate(Guid userId);
    Task<Result> ConfirmEmail(Guid userId, string token);
    Task<Result> UpdateEmail(UpdateEmailModel model);
    Task<Result> UpdateRole(UpdateRoleModel model);
    Task<IReadOnlyList<UserRoleModel>> GetUsersRoles(IEnumerable<Guid> userIds);
    Task<IReadOnlyList<AgencyMemberModel>> GetAgencyMembers(IEnumerable<Guid> agencyIds);
    Task<IReadOnlyList<AgencyMemberModel>> GetUsersAgencies(IEnumerable<Guid> userIds);
}
