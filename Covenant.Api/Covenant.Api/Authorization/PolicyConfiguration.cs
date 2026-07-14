using Covenant.Common.Constants;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Api.Authorization
{
    public static class PolicyConfiguration
    {
        public const string Agency = "Agency";
        public const string Recruiting = "Recruiting";
        public const string Company = "Company";
        public const string Worker = "Worker";
        public const string Request = "Request";
        public const string Covenant = "Covenant";
        public const string AgencyOrCompany = "AgencyOrCompany";
        public const string AgencyOrWorker = "AgencyOrWorker";
        public const string Accounting = "Accounting";
        public const string SuperAdmin = "SuperAdmin";
        public const string Sales = "Sales";

        public static IServiceCollection AddPolices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Agency, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.AgencyStaff));
                options.AddPolicy(Recruiting, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.RecruitingAccess));
                options.AddPolicy(Sales, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.SalesAccess));
                options.AddPolicy(Company, b => b.RequireAuthenticatedUser().RequireAssertion(a =>
                    a.User.IsInRole(CovenantConstants.Role.Company) ||
                    a.User.IsInRole(CovenantConstants.Role.CompanyUser)));
                options.AddPolicy(Worker, b => b.RequireAuthenticatedUser().RequireAssertion(a => a.User.IsInRole(CovenantConstants.Role.Worker)));
                options.AddPolicy(Covenant, b => b.RequireAuthenticatedUser().RequireClaim("all2job", "all2job"));
                options.AddPolicy(AgencyOrCompany, b => b.RequireAuthenticatedUser().RequireRole(
                    [.. CovenantConstants.Role.RecruitingAccess, CovenantConstants.Role.Company]));
                options.AddPolicy(AgencyOrWorker, b => b.RequireAuthenticatedUser().RequireRole(
                    [.. CovenantConstants.Role.RecruitingAccess, CovenantConstants.Role.Worker]));
                options.AddPolicy(Accounting, b => b.RequireAuthenticatedUser().RequireAssertion(a => a.User.IsAccountingManager()));
                options.AddPolicy(SuperAdmin, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.SuperAdmin));
                options.AddPolicy(Request, b => b.RequireAuthenticatedUser());
            });
            return services;
        }
    }
}
