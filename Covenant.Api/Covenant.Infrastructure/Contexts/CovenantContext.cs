using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Accounting;
using Covenant.Deductions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Context
{
    public class CovenantContext : DbContext
    {
        public CovenantContext()
        {
        }

        public CovenantContext(DbContextOptions<CovenantContext> options) : base(options)
        {
        }

        public DbSet<Agency> Agencies { get; set; }
        public DbSet<WorkerProfile> WorkerProfile { get; set; }
        public DbSet<WorkerComment> WorkerComment { get; set; }
        public DbSet<WorkerRequest> WorkerRequest { get; set; }
        public DbSet<WorkerRequestNote> WorkerRequestNote { get; set; }
        public DbSet<TimeSheet> TimeSheet { get; set; }
        public DbSet<TimeSheetPhoto> TimeSheetPhoto { get; set; }
        public DbSet<WorkerProfileHoliday> WorkerProfileHoliday { get; set; }
        public DbSet<WorkerProfileNote> WorkerProfileNote { get; set; }
        public DbSet<WorkerProfileLicense> WorkerProfileLicenses { get; set; }
        public DbSet<Common.Entities.Request.TimeSheetTotal> TimeSheetTotal { get; set; }
        public DbSet<TimeSheetTotalPayroll> TimeSheetTotalPayroll { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<CompanyProfile> CompanyProfile { get; set; }
        public DbSet<CompanyProfileLocation> CompanyProfileLocations { get; set; }
        public DbSet<CompanyProfileJobPositionRate> CompanyProfileJobPositionRate { get; set; }
        public DbSet<CompanyProfileInvoiceNotes> CompanyProfileInvoiceNotes { get; set; }
        public DbSet<CompanyProfileHoliday> CompanyProfileHoliday { get; set; }
        public DbSet<CompanyUser> CompanyUser { get; set; }
        public DbSet<CompanyProfileDocument> CompanyProfileDocuments { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<RequestCancellationDetail> RequestCancellationDetail { get; set; }
        public DbSet<RequestFinalizationDetail> RequestFinalizationDetail { get; set; }
        public DbSet<RequestRequestedBy> RequestRequestedBy { get; set; }
        public DbSet<RequestReportTo> RequestReportTo { get; set; }
        public DbSet<RequestNote> RequestNotes { get; set; }
        public DbSet<RequestRecruiter> RequestRecruiter { get; set; }
        public DbSet<RequestSkill> RequestSkill { get; set; }
        public DbSet<RequestApplicant> RequestApplicant { get; set; }
        public DbSet<RequestComission> RequestComissions { get; set; }
        public DbSet<RequestCompanyUser> RequestCompanyUsers { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceUSA> InvoiceUSA { get; set; }
        public DbSet<InvoiceUSATimeSheetTotal> InvoiceUSATimeSheetTotals { get; set; }
        public DbSet<InvoiceTotal> InvoiceTotals { get; set; }
        public DbSet<CompanyProfileInvoiceRecipient> CompanyProfileInvoiceRecipient { get; set; }
        public DbSet<SkipPayrollNumber> SkipPayrollNumbers { get; set; }
        public DbSet<NextNumberModel> NextNumber { get; set; }
        public DbSet<Availability> Availability { get; set; }
        public DbSet<AvailabilityTime> AvailabilityTime { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Day> Day { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<IdentificationType> IdentificationType { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<Lift> Lift { get; set; }
        public DbSet<Province> Province { get; set; }
        public DbSet<ProvinceTax> ProvinceTaxes { get; set; }
        public DbSet<ProvinceSetting> ProvinceSettings { get; set; }
        public DbSet<WsibGroup> WsibGroup { get; set; }
        public DbSet<Industry> Industry { get; set; }
        public DbSet<JobPosition> JobPosition { get; set; }
        public DbSet<ReasonCancellationRequest> ReasonCancellationRequest { get; set; }
        public DbSet<Holiday> Holiday { get; set; }
        public DbSet<CovenantFile> CovenantFile { get; set; }
        public DbSet<CppWeekly> CppWeekly { get; set; }
        public DbSet<CppBiWeekly> CppBiWeekly { get; set; }
        public DbSet<CppMonthly> CppMonthly { get; set; }
        public DbSet<CppSemiMonthly> CppSemiMonthly { get; set; }
        public DbSet<TaxWeekly> TaxWeekly { get; set; }
        public DbSet<FederalTaxBiWeekly> FederalTaxBiWeekly { get; set; }
        public DbSet<FederalTaxSemiMonthly> FederalTaxSemiMonthly { get; set; }
        public DbSet<FederalTaxMonthly> FederalTaxMonthly { get; set; }
        public DbSet<ProvincialTaxWeekly> ProvincialTaxWeekly { get; set; }
        public DbSet<ProvincialTaxBiWeekly> ProvincialTaxBiWeekly { get; set; }
        public DbSet<ProvincialTaxSemiMonthly> ProvincialTaxSemiMonthly { get; set; }
        public DbSet<ProvincialTaxMonthly> ProvincialTaxMonthly { get; set; }
        public DbSet<NotificationType> NotificationType { get; set; }
        public DbSet<UserNotificationType> UserNotificationType { get; set; }
        public DbSet<PayStub> PayStub { get; set; }
        public DbSet<PayStubItem> PayStubItem { get; set; }
        public DbSet<PayStubWageDetail> PayStubWageDetail { get; set; }
        public DbSet<PayStubOtherDeduction> PayStubOtherDeduction { get; set; }
        public DbSet<ReportSubcontractor> ReportSubcontractor { get; set; }
        public DbSet<ReportSubContractorOtherDeduction> ReportSubContractorOtherDeduction { get; set; }
        public DbSet<ReportSubcontractorWageDetail> ReportSubcontractorWageDetail { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<CandidatePhone> CandidatePhones { get; set; }
        public DbSet<CandidateDocument> CandidateDocuments { get; set; }
        public DbSet<CandidateNote> CandidateNotes { get; set; }
        public DbSet<AgencyPersonnel> AgencyPersonnel { get; set; }
        public DbSet<AgencyLocation> AgencyLocations { get; set; }
        public DbSet<WorkerProfileTaxCategory> WorkerProfileTaxCategories { get; set; }
        public DbSet<PayStubHistory> PayStubHistories { get; set; }
        public DbSet<TimesheetHistory> TimesheetHistories { get; set; }
        public DbSet<CompanyProfileContactPerson> CompanyProfileContactPersons { get; set; }
        public DbSet<CompanyProfileNote> CompanyProfileNotes { get; set; }
        public DbSet<WorkerProfileOtherDocument> WorkerProfileOtherDocuments { get; set; }
        public DbSet<WorkerProfileCertificate> WorkerProfileCertificates { get; set; }
        public DbSet<PayStubPublicHoliday> PayStubPublicHolidays { get; set; }
        public DbSet<ReportSubcontractorPublicHoliday> ReportSubcontractorPublicHolidays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            if (Database.IsNpgsql())
            {
                modelBuilder.AddPostgresFunctions();
            }
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = 0;
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    result = await base.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    foreach (var entity in ex.Entries)
                    {
                        var databaseValue = entity.GetDatabaseValues();
                        if (databaseValue == null)
                        {
                            entity.State = EntityState.Added;
                        }
                    }
                }

            } while (saveFailed);
            return result;
        }
    }
}