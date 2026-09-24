using Covenant.Common.Interfaces;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Http;

namespace Covenant.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid GetUserId() => httpContextAccessor.HttpContext.User.GetUserId();

    public Guid GetCompanyId() => httpContextAccessor.HttpContext.User.GetCompanyId();

    public Guid GetAgencyId() => httpContextAccessor.HttpContext.User.GetAgencyId();

    public Guid GetAgencyPersonnelId() => httpContextAccessor.HttpContext.User.GetAgencyPersonnelId();

    public IEnumerable<Guid> GetAgencyIds() => httpContextAccessor.HttpContext.User.GetAgencyIds();

    public string GetNickname() => httpContextAccessor.HttpContext?.User?.GetNickname();

    public bool IsAdmin() => httpContextAccessor.HttpContext.User.IsAdmin();

    public bool IsAgencyStaff() => httpContextAccessor.HttpContext.User.IsAgencyStaff();

    public bool IsSales() => httpContextAccessor.HttpContext.User.IsSales();

    public bool IsSuperAdmin() => httpContextAccessor.HttpContext.User.IsSuperAdmin();
}
