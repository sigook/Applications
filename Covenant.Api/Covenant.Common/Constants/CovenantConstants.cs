namespace Covenant.Common.Constants
{
    public static class CovenantConstants
    {
        public const string DefaultRegion = "CA";
        public const string ExcelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string CompanyId = "companyId";
        public const string AgencyId = "agencyId";
        public const string AgencyIds = "agencyIds";

        public static class TypeUser
        {
            public const string Agency = "agency";
            public const string Worker = "worker";
            public const string Company = "company";
        }

        public static class Role
        {
            public const string Company = "company";
            public const string CompanyUser = "company.user";
            public const string AgencyPersonnel = "agency.personnel";
            public const string Worker = "worker";
            public const string Agency = "agency";
        }

        public static class Validation
        {
            public const long MaximumFileSize = 11000000;
            public const int FullNameMinimumLength = 2;
            public const int FullNameMaximumLength = 60;
            public const int BusinessNameMinimumLength = 2;
            public const int BusinessNameMaximumLength = 50;
            public const int ContactInformationMaximum = 10;
            public const int AsapRateMinimum = 0;
            public const int AsapRateMaximum = 999999;

            public const int HstNumberMinimumLength = 15;
            public const int HstNumberMaximumLength = 20;
            public const int BusinessNumberMinimumLength = 9;
            public const int BusinessNumberMaximumLength = 15;

            public const int WsibMaximumLength = 60;
            public const int PasswordMinimumLength = 6;
            public const int PasswordMaximumLength = 100;
            public const int MobilePhoneMinimumLength = 10;
            public const int MobilePhoneMaximumLength = 22;
            public const int PhoneMinimumLength = 6;
            public const int PhoneMaximumLength = 22;
            public const int PhoneExtMinimum = 0;
            public const int PhoneExtMaximum = 99999999;
            public const int TitleMinimumLength = 1;
            public const int TitleMaximumLength = 50;
            public const int FirstNameMinLength = 1;
            public const int FirstNameMaxLength = 20;
            public const int MiddleNameMinimumLength = 1;
            public const int MiddleNameMaximumLength = 20;
            public const int LastNameMinLength = 2;
            public const int LastNameMaxLength = 20;
            public const int PositionMinimumLength = 3;
            public const int PositionMaximumLength = 30;

            public const int MaximumLengthRequirements = 50000;
            public const int MinimumLengthDescription = 5;
            public const int MaximumLengthDescription = 5000;
            public const int MinimumHourPrice = 1;
            public const int MaximumHourPrice = 9999999;
            public const double MaximumFirstDay = 365;

            public const double MaximumHoursWeekRequest = 48D;
            public const int MaximumLengthRepeat = 50;
            public const double MinimumHoursDay = 1;

            public const int MaximumWorkShiftTime = 365;
            public const int MinimumWorkShiftTime = 1;

            public const int ApprovedHoursMinimum = 0;
            public const int ApprovedHoursMaximum = 24;
            public const int CommentMaximum = 5000;

            public const int CompanyMinLength = 2;
            public const int CompanyMaxLength = 50;
            public const int SupervisorMinLength = 2;
            public const int SupervisorMaxLength = 50;
            public const int DutiesMinLength = 2;
            public const int DutiesMaxLength = 5000;

            public const int BirthdayMinimumAge = 18;
            public const int SocialInsuranceMaxLength = 15;
            public const int SocialInsuranceMinLength = 9;
            public const int HealthProblemMaxLength = 50;
            public const int ContactEmergencyMaxLength = 20;
            public const int ContactEmergencyMinLength = 2;
            public const int JobExperiencesLessThanOrEqualTo = 30;

            public const decimal RateMinimumLength = 0.1m;
            public const decimal RateMaximumLength = 50;
            public const double MaximumMissingHours = 6;

            public const string PhonePattern = @"\s*(?:\+?(\d{1,3}))?([-. (]*(\d{3})[-. )]*)?((\d{3})[-. ]*(\d{2,4})(?:[-.x ]*(\d+))?)\s*";
            public const string WebpagePattern = @"^((http://)|(https://))?([a-zA-Z0-9]+[.])+[a-zA-Z]{2,4}(:\d+)?(/[~_.\-a-zA-Z0-9=&%@:]+)*\??[~_.\-a-zA-Z0-9=&%@:]*$";
        }

        public static class PublicHolidays
        {
            public const int TwentyDays = 20;
            public const double LimitForWorkerWorkedHours = 44 * 4;
        }

        public static class Invoice
        {
            public const string RegularLabel = "Regular";
            public const string MissingDifferentRateLabel = "Missing different rate";
            public const string MissingOvertimeDifferentRateLabel = "Missing Overtime different rate";
            public const string MissingLabel = "Missing";
            public const string MissingOvertimeLabel = "Missing Overtime";
            public const string NightShiftLabel = "Night shift";
            public const string HolidayLabel = "Holiday";
            public const string OvertimeLabel = "Overtime";
        }
    }
}
