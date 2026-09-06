using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Contexts;

public class CovenantContext : DbContext
{
    public CovenantContext()
    {
    }

    public CovenantContext(DbContextOptions<CovenantContext> options) : base(options)
    {
    }

    public DbSet<Agency> Agencies { get; set; }
    public DbSet<WorkerProfile> WorkerProfiles { get; set; }
    public DbSet<WorkerComment> WorkerComments { get; set; }
    public DbSet<WorkerRequest> WorkerRequests { get; set; }
    public DbSet<WorkerRequestNote> WorkerRequestNotes { get; set; }
    public DbSet<TimeSheet> TimeSheets { get; set; }
    public DbSet<WorkerProfileHoliday> WorkerProfileHolidays { get; set; }
    public DbSet<WorkerProfileNote> WorkerProfileNotes { get; set; }
    public DbSet<WorkerProfileLicense> WorkerProfileLicenses { get; set; }
    public DbSet<TimeSheetTotal> TimeSheetTotals { get; set; }
    public DbSet<TimeSheetTotalPayroll> TimeSheetTotalPayrolls { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    public DbSet<CompanyProfileLocation> CompanyProfileLocations { get; set; }
    public DbSet<CompanyProfileJobPositionRate> CompanyProfileJobPositionRates { get; set; }
    public DbSet<CompanyProfileInvoiceNotes> CompanyProfileInvoiceNotes { get; set; }
    public DbSet<CompanyUser> CompanyUsers { get; set; }
    public DbSet<CompanyProfileDocument> CompanyProfileDocuments { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestCancellationDetail> RequestCancellationDetails { get; set; }
    public DbSet<RequestFinalizationDetail> RequestFinalizationDetails { get; set; }
    public DbSet<RequestRequestedBy> RequestRequestedBys { get; set; }
    public DbSet<RequestReportTo> RequestReportTos { get; set; }
    public DbSet<RequestNote> RequestNotes { get; set; }
    public DbSet<RequestRecruiter> RequestRecruiters { get; set; }
    public DbSet<RequestSource> RequestSources { get; set; }
    public DbSet<RequestSkill> RequestSkills { get; set; }
    public DbSet<RequestComplianceItem> RequestComplianceItems { get; set; }
    public DbSet<RequestApplicant> RequestApplicants { get; set; }
    public DbSet<RequestApplicantComplianceItem> RequestApplicantComplianceItems { get; set; }
    public DbSet<RequestComission> RequestComissions { get; set; }
    public DbSet<RequestCompanyUser> RequestCompanyUsers { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceUSA> InvoicesUSA { get; set; }
    public DbSet<InvoiceUSAItem> InvoiceUSAItems { get; set; }
    public DbSet<InvoiceUSATimeSheetTotal> InvoiceUSATimeSheetTotals { get; set; }
    public DbSet<InvoiceTotal> InvoiceTotals { get; set; }
    public DbSet<InvoiceAdditionalDetail> InvoiceAdditionalDetails { get; set; }
    public DbSet<CompanyProfileInvoiceRecipient> CompanyProfileInvoiceRecipients { get; set; }
    public DbSet<SkipPayrollNumber> SkipPayrollNumbers { get; set; }
    public DbSet<NextNumberModel> NextNumber { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<AvailabilityTime> AvailabilityTimes { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Day> Days { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<IdentificationType> IdentificationTypes { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Lift> Lifts { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<LocationTax> LocationTaxes { get; set; }
    public DbSet<ProvinceSetting> ProvinceSettings { get; set; }
    public DbSet<WsibGroup> WsibGroups { get; set; }
    public DbSet<Industry> Industries { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<ReasonCancellationRequest> ReasonCancellationRequests { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<CovenantFile> CovenantFiles { get; set; }
    public DbSet<CovenantNote> CovenantNotes { get; set; }
    public DbSet<CppDeduction> CppDeductions { get; set; }
    public DbSet<TaxDeduction> TaxDeductions { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<UserNotificationType> UserNotificationTypes { get; set; }
    public DbSet<PayStub> PayStubs { get; set; }
    public DbSet<PayStubItem> PayStubItems { get; set; }
    public DbSet<PayStubWageDetail> PayStubWageDetails { get; set; }
    public DbSet<PayStubOtherDeduction> PayStubOtherDeductions { get; set; }
    public DbSet<ReportSubcontractor> ReportSubcontractors { get; set; }
    public DbSet<ReportSubContractorOtherDeduction> ReportSubContractorOtherDeductions { get; set; }
    public DbSet<ReportSubcontractorWageDetail> ReportSubcontractorWageDetails { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Runner> Runners { get; set; }
    public DbSet<RunnerStatusHistory> RunnerStatusHistories { get; set; }
    public DbSet<RunnerInterview> RunnerInterviews { get; set; }
    public DbSet<CandidateSkill> CandidateSkills { get; set; }
    public DbSet<CandidatePhone> CandidatePhones { get; set; }
    public DbSet<CandidateDocument> CandidateDocuments { get; set; }
    public DbSet<CandidateNote> CandidateNotes { get; set; }
    public DbSet<AgencyPersonnel> AgencyPersonnel { get; set; }
    public DbSet<AgencyLocation> AgencyLocations { get; set; }
    public DbSet<WorkerProfileTaxCategory> WorkerProfileTaxCategories { get; set; }
    public DbSet<PayStubHistory> PayStubHistories { get; set; }
    public DbSet<TimesheetHistory> TimesheetHistories { get; set; }
    public DbSet<CompanyProfileContactPerson> CompanyProfileContactPeople { get; set; }
    public DbSet<CompanyProfileNote> CompanyProfileNotes { get; set; }
    public DbSet<WorkerProfileOtherDocument> WorkerProfileOtherDocuments { get; set; }
    public DbSet<WorkerProfileCertificate> WorkerProfileCertificates { get; set; }
    public DbSet<PayStubPublicHoliday> PayStubPublicHolidays { get; set; }
    public DbSet<ReportSubcontractorPublicHoliday> ReportSubcontractorPublicHolidays { get; set; }
    public DbSet<CompanyInteraction> CompanyInteractions { get; set; }
    public DbSet<Deal> Deals { get; set; }
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