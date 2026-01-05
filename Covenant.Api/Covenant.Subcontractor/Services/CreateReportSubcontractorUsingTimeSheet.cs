using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Subcontractor.Utils;
using Covenant.TimeSheetTotal.Models;
using Covenant.TimeSheetTotal.Services;

namespace Covenant.Subcontractor.Services;

public class CreateReportSubcontractorUsingTimeSheet
{
    private readonly TimeLimits _timeLimits;
    private readonly Rates _rates;
    private readonly ICatalogRepository _catalogRepository;
    private readonly ISubcontractorRepository _subcontractorRepository;
    private readonly ISubContractorPublicHolidays _subContractorPublicHolidays;
    private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

    public CreateReportSubcontractorUsingTimeSheet(TimeLimits timeLimits,
        Rates rates,
        ICatalogRepository catalogRepository,
        ISubcontractorRepository subcontractorRepository,
        ISubContractorPublicHolidays subContractorPublicHolidays)
    {
        _timeLimits = timeLimits;
        _rates = rates;
        _catalogRepository = catalogRepository;
        _subcontractorRepository = subcontractorRepository;
        _subContractorPublicHolidays = subContractorPublicHolidays;
    }

    public async Task<Result<List<ReportSubcontractor>>> Create(IEnumerable<TimeSheetApprovedPayrollModel> list)
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            var reports = new List<ReportSubcontractor>();
            IReadOnlyCollection<IGrouping<Guid, TimeSheetApprovedPayrollModel>> timeSheetGroupByWorker = list.GroupBy(c => c.WorkerId).ToList();
            foreach (var workerTimeSheet in timeSheetGroupByWorker)
            {
                var first = workerTimeSheet.First();
                var workerProfileId = first.WorkerProfileId;
                var timeSheetByWeek = workerTimeSheet.GroupTimeSheetByWeek();
                foreach (var timeSheet in timeSheetByWeek)
                {
                    List<(ITimeSheetTotal tst, TotalDailyWage totalDailyWage)> totals = timeSheet.TotalizatorParams()
                        .GetPayStubTotals(_rates, _timeLimits.MaxHoursWeek, first.OvertimeStartsAfter);
                    IReadOnlyCollection<DateTime> holidaysInWeek = await _catalogRepository.GetHolidaysInWeek(timeSheet.First().Date);
                    var publicHolidays = new List<ReportSubcontractorPublicHoliday>();
                    if (holidaysInWeek != null && holidaysInWeek.Any())
                    {
                        publicHolidays.AddRange(await _subContractorPublicHolidays.GetSubcontractorWorkerHolidays(holidaysInWeek, workerProfileId));
                    }
                    DateTime dateWorkBegins = timeSheet.Min(m => m.Date);
                    DateTime dateWorkEnd = timeSheet.Max(m => m.Date);
                    decimal otherEarnings = timeSheet.Sum(m => m.BonusOrOthers);//TODO include in report

                    var otherDeductions = OtherDeductions(timeSheet);
                    ReportSubcontractor report = ReportSubContractorBuilder.Report()
                        .WithWorkerProfileId(workerProfileId)
                        .WithWorkBeginning(dateWorkBegins)
                        .WithWorkEnding(dateWorkEnd)
                        .WithWageDetails(totals.Select(t => t.totalDailyWage.ToSubcontractorWageDetail(t.tst)))
                        .WithPublicHolidaysToPay(publicHolidays)
                        .WithOtherDeductions(otherDeductions).Build();

                    await _subcontractorRepository.Create(report);
                    reports.Add(report);
                }
            }
            await _subcontractorRepository.SaveChangesAsync();
            return Result.Ok(reports);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private static IEnumerable<ReportSubContractorOtherDeduction> OtherDeductions(IEnumerable<TimeSheetApprovedPayrollModel> workerTimeSheet) =>
        workerTimeSheet
            .Where(m => m.DeductionsOthers > 0)
            .Select(d => ReportSubContractorOtherDeduction.CreateDefaultDeduction(d.DeductionsOthers, d.DeductionsOthersDescription));
}