using Covenant.Common.Constants;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Api.Authorization;

public static class PolicyConfiguration
{
    public const string Agency = "Agency";
    public const string Recruiting = "Recruiting";
    public const string Company = "Company";
    public const string Worker = "Worker";
    public const string Admin = "Admin";
    public const string SuperAdmin = "SuperAdmin";
    public const string Sales = "Sales";

    public static IServiceCollection AddPolices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Agency, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.AgencyStaff));
            options.AddPolicy(Recruiting, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.RecruitingAccess));
            options.AddPolicy(Sales, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.SalesAccess));
            options.AddPolicy(Company, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.Company, CovenantConstants.Role.CompanyUser));
            options.AddPolicy(Worker, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.Worker));
            options.AddPolicy(Admin, b => b.RequireAuthenticatedUser().RequireAssertion(a => a.User.IsAdmin()));
            options.AddPolicy(SuperAdmin, b => b.RequireAuthenticatedUser().RequireRole(CovenantConstants.Role.SuperAdmin));
        });
        return services;
    }
}
