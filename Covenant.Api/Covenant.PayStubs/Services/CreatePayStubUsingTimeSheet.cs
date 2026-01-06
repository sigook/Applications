using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Worker;
using Covenant.Deductions.Services;
using Covenant.PayStubs.Utils;
using Covenant.TimeSheetTotal.Models;
using Covenant.TimeSheetTotal.Services;

namespace Covenant.PayStubs.Services;

public class CreatePayStubUsingTimeSheet
{
    private readonly TimeLimits _timeLimits;
    private readonly Rates _rates;
    private readonly ICatalogRepository _catalogRepository;
    private readonly IPayrollDeductionsAndContributionsCalculator _deductionsAndContributionsCalculator;
    private readonly IPayStubPublicHolidays _payStubPublicHolidays;
    private readonly IPayStubRepository payStubRepository;
    private readonly IWorkerRepository workerRepository;
    private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);


    public CreatePayStubUsingTimeSheet(
        TimeLimits timeLimits,
        Rates rates,
        ICatalogRepository catalogRepository,
        IPayrollDeductionsAndContributionsCalculator deductionsAndContributionsCalculator,
        IPayStubPublicHolidays payStubPublicHolidays,
        IPayStubRepository payStubRepository,
        IWorkerRepository workerRepository)
    {
        _timeLimits = timeLimits;
        _rates = rates;
        _catalogRepository = catalogRepository;
        _deductionsAndContributionsCalculator = deductionsAndContributionsCalculator;
        _payStubPublicHolidays = payStubPublicHolidays;
        this.payStubRepository = payStubRepository;
        this.workerRepository = workerRepository;
        PayStubs = new List<PayStub>();
    }

    public List<PayStub> PayStubs { get; }

    public async Task<Result> Create(IEnumerable<TimeSheetApprovedPayrollModel> list)
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            var timeSheetGroupByWorker = list.GroupBy(c => c.WorkerId).ToList();
            var nextPayStubNumbers = await payStubRepository.GetNextPayStubNumbers(timeSheetGroupByWorker.Count);
            var payStubNumbers = new Queue<NextNumberModel>(nextPayStubNumbers);
            foreach (var workerTimeSheet in timeSheetGroupByWorker)
            {
                long payStubNumber = payStubNumbers.Dequeue().NextNumber;
                var rPayStub = await CreatePayStub(workerTimeSheet.First().WorkerProfileId, workerTimeSheet.ToList(), payStubNumber);
                if (!rPayStub) return rPayStub;
                await payStubRepository.Create(rPayStub.Value);
                PayStubs.Add(rPayStub.Value);
            }
            await payStubRepository.SaveChangesAsync();
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return Result.Ok();
    }

    private async Task<Result<PayStub>> CreatePayStub(Guid workerProfileId, IReadOnlyCollection<TimeSheetApprovedPayrollModel> timeSheets, long payStubNumber)
    {
        var first = timeSheets.First();
        var totals = new List<(ITimeSheetTotal tst, TotalDailyWage totalDailyWage)>();
        var publicHolidays = new List<PayStubPublicHoliday>();
        var timeSheetByWeek = timeSheets.GroupTimeSheetByWeek();
        foreach (var timeSheet in timeSheetByWeek)
        {
            var totalizatorParams = timeSheet.TotalizatorParams();
            var payStubTotals = totalizatorParams.GetPayStubTotals(_rates, _timeLimits.MaxHoursWeek, first.OvertimeStartsAfter);
            totals.AddRange(payStubTotals);
            var holidaysInWeek = await _catalogRepository.GetHolidaysInWeek(timeSheet.First().Date);
            if (holidaysInWeek is null || !holidaysInWeek.Any()) continue;
            var rPublicHolidays = await _payStubPublicHolidays.GetWorkerPublicHolidays(holidaysInWeek, workerProfileId);
            if (!rPublicHolidays) return Result.Fail<PayStub>(rPublicHolidays.Errors);
            publicHolidays.AddRange(rPublicHolidays.Value);
        }

        var rPayStubItems = totals.Select(t => t.totalDailyWage).ToList().ToPayStubItems();
        var itemsErrors = rPayStubItems.Where(r => r.IsFailure).SelectMany(r => r.Errors).ToList();
        if (itemsErrors.Any()) return Result.Fail<PayStub>(itemsErrors);
        var payStubItems = rPayStubItems.Select(r => r.Value).ToList();
        var otherEarnings = timeSheets.Where(w => w.BonusOrOthers > 0)
            .GroupBy(o => o.BonusOrOthersDescription)
            .Select(os => PayStubItem.CreateBonusOthersItem(os.Sum(t => t.BonusOrOthers), os.Key))
            .Where(r => r)
            .Select(r => r.Value)
            .ToList();
        var otherDeductions = timeSheets
            .Where(m => m.DeductionsOthers > 0)
            .Select(d => PayStubOtherDeduction.CreateDefaultDeduction(d.DeductionsOthers, d.DeductionsOthersDescription));
        var reimbursements = timeSheets.Where(ts => ts.Reimbursements > 0)
            .GroupBy(ts => ts.ReimbursementsDescription)
            .Select(ts => PayStubItem.CreateReimbursements(ts.Sum(r => r.Reimbursements), ts.Key))
            .Where(ts => ts)
            .Select(ts => ts.Value)
            .ToList();
        payStubItems.AddRange(otherEarnings);
        var minDate = timeSheets.Min(m => m.Date);
        var maxDate = timeSheets.Max(m => m.Date);

        var wageDetails = totals.Select(t => t.totalDailyWage.ToWageDetail(t.tst)).ToList();
        return await PayStubBuilder.PayStub(_rates, _deductionsAndContributionsCalculator, workerRepository)
            .WithPayStubNumber(payStubNumber)
            .WithWorkerProfileId(workerProfileId)
            .WithTypeOfWork(first.TypeOfWork)
            .WithWorkBeginning(minDate)
            .WithWorkEnding(maxDate)
            .WithCreationDate(DateTime.Now)
            .WithItems(payStubItems)
            .WithWageDetails(wageDetails)
            .WithPublicHolidaysToPay(publicHolidays)
            .WithOtherDeductions(otherDeductions)
            .WithReimbursement(reimbursements)
            .PayVacations()
            .Build();
    }
}