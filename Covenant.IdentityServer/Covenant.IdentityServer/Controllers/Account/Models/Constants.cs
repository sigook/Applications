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

        public static Claim ClaimCompanyId(Guid id) => new Claim(CompanyId, id.ToString());
        public static Claim ClaimAgencyId(Guid id) => new Claim(AgencyId, id.ToString());

        public static Claim All2JobClaim() => new Claim(All2job, All2job);

        public static Claim AgencyClaim() => new Claim(Agency, Agency);

        public static Claim WorkerClaim() => new Claim(Worker, Worker);
    }
}