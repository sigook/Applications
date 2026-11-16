namespace Covenant.Common.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    Guid GetCompanyId();
    Guid GetAgencyId();
    List<Guid> GetAgencyIds();
    string GetNickname();
    bool IsAdmin();
    bool IsAgencyStaff();
    bool IsCompanyUser();
    bool IsSales();
    bool IsSuperAdmin();
}
