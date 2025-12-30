using Covenant.Common.Constants;

namespace Covenant.Api.Authorization
{
    public static class PolicyConfiguration
    {
        public const string Agency = "Agency";
        public const string Company = "Company";
        public const string Worker = "Worker";
        public const string Request = "Request";
        public const string Covenant = "Covenant";
        public const string AgencyOrCompany = "AgencyOrCompany";
        public const string AgencyOrWorker = "AgencyOrWorker";

        public static IServiceCollection AddPolices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Agency, b => b.RequireAuthenticatedUser().RequireAssertion(a => a.User.IsInRole(CovenantConstants.Role.AgencyPersonnel)));
                options.AddPolicy(Company, b => b.RequireAuthenticatedUser().RequireAssertion(a =>
                    a.User.IsInRole(CovenantConstants.Role.Company) ||
                    a.User.IsInRole(CovenantConstants.Role.CompanyUser)));
                options.AddPolicy(Worker, b => b.RequireAuthenticatedUser().RequireAssertion(a => a.User.IsInRole(CovenantConstants.Role.Worker)));
                options.AddPolicy(Covenant, b => b.RequireAuthenticatedUser().RequireClaim("all2job", "all2job"));
                options.AddPolicy(AgencyOrCompany, b => b.RequireAuthenticatedUser().RequireAssertion(c =>
                    c.User.IsInRole(CovenantConstants.Role.AgencyPersonnel) ||
                    c.User.IsInRole(CovenantConstants.Role.Company)));
                options.AddPolicy(AgencyOrWorker, b => b.RequireAuthenticatedUser().RequireAssertion(c =>
                    c.User.IsInRole(CovenantConstants.Role.AgencyPersonnel) ||
                    c.User.IsInRole(CovenantConstants.Role.Worker)));
                options.AddPolicy(Request, b => b.RequireAuthenticatedUser());
            });
            return services;
        }
    }
}