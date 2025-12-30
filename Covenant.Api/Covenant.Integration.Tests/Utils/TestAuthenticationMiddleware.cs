using Covenant.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Covenant.Integration.Tests.Utils
{
    public static class TestAuthenticationMiddleware
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> options, string scheme = default) =>
            builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(scheme ?? "Bearer", "Bearer", options);

        public static AuthenticationBuilder AddTestAuthenticationBuilder(this IServiceCollection services, string scheme = default) => services.AddAuthentication(scheme ?? "Bearer");

        public static void AddWorkerRole(this TestAuthenticationOptions options) =>
            options.Identity.AddClaim(new Claim(ClaimTypes.Role, CovenantConstants.Role.Worker));

        public static void AddCompanyRole(this TestAuthenticationOptions options) =>
            options.Identity.AddClaim(new Claim(ClaimTypes.Role, CovenantConstants.Role.Company));

        public static void AddCompanyUserRole(this TestAuthenticationOptions options) =>
            options.Identity.AddClaim(new Claim(ClaimTypes.Role, CovenantConstants.Role.CompanyUser));

        public static void AddAgencyPersonnelRole(this TestAuthenticationOptions options, Guid? agencyId = default)
        {
            options.Identity.AddClaim(new Claim(ClaimTypes.Role, CovenantConstants.Role.AgencyPersonnel));
            if (agencyId.HasValue)
                options.Identity.AddClaim(new Claim(CovenantConstants.AgencyId, agencyId.Value.ToString()));
        }

        public static void AddSub(this TestAuthenticationOptions options, Guid sub) =>
            options.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub.ToString()));

        public static void AddName(this TestAuthenticationOptions options, string name = "user@mail.com") =>
            options.Identity.AddClaim(new Claim("name", name));
    }
}