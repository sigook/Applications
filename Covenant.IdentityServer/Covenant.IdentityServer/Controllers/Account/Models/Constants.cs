using System.Security.Claims;

namespace Covenant.IdentityServer.Controllers.Account.Models
{
    public static class Constants
    {
        public const string Agency = "agency";
        public const string Company = "company";
        public const string Worker = "worker";
        public const string Manager = "manager";
        private const string All2job = "all2job";
        public static readonly string[] UserTypes = { Agency, Company, Worker, Manager, All2job };

        public const string CompanyId = "companyId";
        public const string AgencyId = "agencyId";

        public static Claim ClaimCompanyId(Guid id) => new(CompanyId, id.ToString());
        public static Claim ClaimAgencyId(Guid id) => new(AgencyId, id.ToString());
    }
}