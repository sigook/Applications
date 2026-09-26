namespace Covenant.Common.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    Guid GetCompanyId();
    Guid GetAgencyId();
    Guid GetAgencyPersonnelId();
    IEnumerable<Guid> GetAgencyIds();
    string GetNickname();
    bool IsAdmin();
    bool IsAgencyStaff();
    bool IsSales();
    bool IsSuperAdmin();
}
