namespace Covenant.Common.Constants;

public static class CovenantConstants
{
    public const string ExcelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string CompanyId = "companyId";
    public const string AgencyId = "agencyId";
    public const string AgencyIds = "agencyIds";
    public const string AgencyPersonnelId = "agencyPersonnelId";

    public static class Role
    {
        public const string SuperAdmin = "superadmin";
        public const string Admin = "admin";
        public const string Recruiting = "recruiting";
        public const string Sales = "sales";
        public const string Company = "company";
        public const string CompanyUser = "company.user";
        public const string Worker = "worker";

        public static readonly string[] RecruitingAccess = [SuperAdmin, Admin, Recruiting];

        public static readonly string[] AgencyStaff = [.. RecruitingAccess, Sales];

        public static readonly string[] SalesAccess = [SuperAdmin, Admin, Sales];

        public static readonly string[] AdminAccess = [SuperAdmin, Admin];

        public static readonly string[] AgencyAssignable = [Admin, Recruiting, Sales];

        public static readonly string[] SuperAdminAssignable = [SuperAdmin, .. AgencyAssignable];
    }

    public static class Validation
    {
        public const int FirstNameMinLength = 1;
        public const int FirstNameMaxLength = 20;
        public const int LastNameMinLength = 2;
        public const int LastNameMaxLength = 20;

        public const int MaximumLengthRequirements = 50000;

        public const int BirthdayMinimumAge = 18;
        public const int HealthProblemMaxLength = 50;
        public const int ContactEmergencyMaxLength = 20;
        public const int ContactEmergencyMinLength = 2;
    }

    public static class PublicHolidays
    {
        public const double LimitForWorkerWorkedHours = 44 * 4;
    }
}
