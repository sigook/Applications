using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Functionals;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils;
using Covenant.Common.Utils.Extensions;
using Covenant.Deductions.Services;
using Covenant.PayStubs.Utils;

namespace Covenant.PayStubs.Services;

[Obsolete]
public class PayStubBuilder :
    IPayStubNumberHolder,
    IWorkerProfileIdHolder,
    IPositionHolder,
    IDateWorkBeginHolder,
    IDateWorkEndsHolder,
    ICreatedAtHolder,
    IPayStubItemsHolder,
    IWageDetailsHolder,
    IPublicHolidaysToPayHolder,
    IOtherDeductionsHolder,
    IReimbursementHolder,
    IPayVacations,
    IPayStubBuilder
{
    private readonly Rates _rates;
    private readonly IPayrollDeductionsAndContributionsCalculator _deductionsCalculator;
    private readonly IWorkerRepository workerRepository;
    private long _number;
    private Guid _workerProfileId;
    private string _position;
    private DateTime _workBegins;
    private DateTime _workEnd;
    private DateTime _createdAt = DateTime.Now;
    private IEnumerable<PayStubWageDetail> _wageDetails = [];
    private IReadOnlyCollection<PayStubPublicHoliday> _holidaysToPay = [];
    private IReadOnlyCollection<PayStubItem> _items = [];
    private bool _payVacations;
    private IEnumerable<PayStubOtherDeduction> _otherDeductions = [];
    private IReadOnlyCollection<PayStubItem> _reimbursements = [];

    private PayStubBuilder(Rates rates, IPayrollDeductionsAndContributionsCalculator deductionsCalculator, IWorkerRepository workerRepository)
    {
        _rates = rates;
        _deductionsCalculator = deductionsCalculator;
        this.workerRepository = workerRepository;
    }

    public static IPayStubNumberHolder PayStub(Rates rates, IPayrollDeductionsAndContributionsCalculator deductionsAndContributionsCalculator, IWorkerRepository workerRepository) =>
        new PayStubBuilder(rates, deductionsAndContributionsCalculator, workerRepository);

    public IWorkerProfileIdHolder WithPayStubNumber(long number) => this.Chain(b => b._number = number);

    public IPositionHolder WithWorkerProfileId(Guid workerProfileId) => this.Chain(b => b._workerProfileId = workerProfileId);

    public IDateWorkBeginHolder WithPosition(string position) => this.Chain(b => b._position = position);

    public IDateWorkEndsHolder WithWorkBeginning(DateTime workBegins) => this.Chain(b =>
    {
        var daysFromSunday = (int)workBegins.DayOfWeek;
        var dateWorkBegins = workBegins.AddDays(-daysFromSunday);
        b._workBegins = dateWorkBegins;
    });

    public ICreatedAtHolder WithWorkEnding(DateTime workEnd) => this.Chain(b =>
    {
        var daysToSaturday = 6 - (int)workEnd.DayOfWeek;
        var dateWorkEnd = workEnd.AddDays(daysToSaturday);
        b._workEnd = dateWorkEnd;
    });

    public IPayStubItemsHolder WithCreationDate(DateTime createdAt) => this.Chain(b => b._createdAt = createdAt);

    public IWageDetailsHolder WithItems(IReadOnlyCollection<PayStubItem> items) => this.Chain(b => b._items = [.. items]);

    public IPublicHolidaysToPayHolder WithWageDetails(IEnumerable<PayStubWageDetail> wageDetails) => this.Chain(b => b._wageDetails = wageDetails);

    public IPublicHolidaysToPayHolder WithoutWageDetails() => this;

    public IOtherDeductionsHolder WithPublicHolidaysToPay(IReadOnlyCollection<PayStubPublicHoliday> publicHolidays) =>
        this.Chain(b => b._holidaysToPay = [.. publicHolidays ?? []]);

    public IOtherDeductionsHolder WithoutPublicHolidaysToPay() => this;

    public IReimbursementHolder WithOtherDeductions(PayStubOtherDeduction deduction) => this.Chain(b => b._otherDeductions = new[] { deduction });

    public IReimbursementHolder WithOtherDeductions(IEnumerable<PayStubOtherDeduction> deductions) => this.Chain(b => b._otherDeductions = deductions);

    public IReimbursementHolder WithNoMoreDeductions() => this;

    public IPayVacations WithReimbursement(IReadOnlyCollection<PayStubItem> items) => this.Chain(psb => psb._reimbursements = new List<PayStubItem>(items));

    public IPayVacations WithoutReimbursement() => this;

    public IPayStubBuilder PayVacations(bool pay = true) => this.Chain(b => b._payVacations = pay);

    public async Task<Result<PayStub>> Build()
    {
        _items = _items.Where(i => i.Total > 0).ToList();
        _reimbursements = _reimbursements.Where(r => r.Total > 0).ToList();
        if (_items.Count == 0) return Result.Fail<PayStub>(ApiResources.There_is_not_enough_information_to_generate_pay_stub);
        if (_workBegins > _workEnd) return Result.Fail<PayStub>("Dates of work: start must be before end");
        var workerProfileTaxCategory = await workerRepository.GetWorkerProfileTaxCategory(_workerProfileId);
        var grossForVacations = _items.Sum(i => i.Total).DefaultMoneyRound();
        var vacations = _payVacations ? PayrollFormulas.Vacations(grossForVacations, _rates.Vacations).DefaultMoneyRound() : decimal.Zero;
        var publicHolidayPay = _holidaysToPay.Sum(r => r.Amount).DefaultMoneyRound();

        if (publicHolidayPay > 0)
        {
            var rHolidayItem = PayStubItem.CreateStatutoryHoliday(publicHolidayPay);
            if (rHolidayItem)
            {
                var items = _items.ToList();
                items.Add(rHolidayItem.Value);
                _items = items;
            }
        }

        var grossPayment = _items.Sum(i => i.Total).DefaultMoneyRound();
        var totalEarnings = grossPayment.Add(vacations).DefaultMoneyRound();
        var numberOfWeeks = _workBegins.GetNumberOfWeeksIn(_workEnd);
        var payrollDeductionsAndContributions = await _deductionsCalculator.CalculateFor(
            totalEarnings,
            numberOfWeeks,
            _workEnd.Year,
            workerProfileTaxCategory);
        var otherDeductions = _otherDeductions.Sum(d => d.Total);
        var totalDeductions = payrollDeductionsAndContributions.Total + otherDeductions;
        var reimbursement = _reimbursements.Sum(r => r.Total).DefaultMoneyRound();
        var totalPaid = decimal.Subtract(totalEarnings, totalDeductions).DefaultMoneyRound().Add(reimbursement);
        var paymentDate = GetPaymentDate();
        var regularWage = _items.GetRegularWage();
        var payStub = new PayStub
        {
            Id = Guid.NewGuid(),
            WorkerProfileId = _workerProfileId,
            PayStubNumber = $"PS-{_number:0000}-{_createdAt:yy}",
            PayStubNumberId = _number,
            Position = _position,
            DateWorkBegins = _workBegins,
            DateWorkEnd = _workEnd,
            PaymentDate = paymentDate,
            RegularWage = regularWage,
            GrossPayment = grossPayment,
            Vacations = vacations,
            TotalEarnings = totalEarnings,
            Cpp = payrollDeductionsAndContributions.Cpp,
            Ei = payrollDeductionsAndContributions.Ei,
            FederalTax = payrollDeductionsAndContributions.FederalTax,
            ProvincialTax = payrollDeductionsAndContributions.ProvincialTax,
            TotalDeductions = totalDeductions,
            TotalPaid = totalPaid,
            CreatedAt = _createdAt,
            WeekEnding = _workEnd.GetWeekEndingCurrentWeek(),
            Items = [.. _items, .. _reimbursements],
            WageDetails = [.. _wageDetails],
            Holidays = [.. _holidaysToPay],
            OtherDeductions = [.. _otherDeductions]
        };
        return Result.Ok(payStub);
    }

    private DateTime GetPaymentDate() => _wageDetails.Any() ? _workEnd.GetPaymentDateForExternalWorkers() : _workEnd.GetPaymentDateForInternalWorkers();
}

internal static class TmpHelper
{
    internal static T Chain<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }
}

public interface IPayStubNumberHolder
{
    IWorkerProfileIdHolder WithPayStubNumber(long number);
}

public interface IWorkerProfileIdHolder
{
    IPositionHolder WithWorkerProfileId(Guid workerProfileId);
}

public interface IPositionHolder
{
    IDateWorkBeginHolder WithPosition(string type);
}

public interface IDateWorkBeginHolder
{
    IDateWorkEndsHolder WithWorkBeginning(DateTime workBegins);
}

public interface IDateWorkEndsHolder
{
    ICreatedAtHolder WithWorkEnding(DateTime workEnd);
}

public interface ICreatedAtHolder
{
    IPayStubItemsHolder WithCreationDate(DateTime createdAt);
}

public interface IPayStubItemsHolder
{
    IWageDetailsHolder WithItems(IReadOnlyCollection<PayStubItem> items);
}

public interface IWageDetailsHolder
{
    IPublicHolidaysToPayHolder WithWageDetails(IEnumerable<PayStubWageDetail> wageDetails);
    IPublicHolidaysToPayHolder WithoutWageDetails();
}

public interface IPublicHolidaysToPayHolder
{
    IOtherDeductionsHolder WithPublicHolidaysToPay(IReadOnlyCollection<PayStubPublicHoliday> publicHolidays);
    IOtherDeductionsHolder WithoutPublicHolidaysToPay();
}

public interface IOtherDeductionsHolder
{
    IReimbursementHolder WithOtherDeductions(PayStubOtherDeduction deduction);
    IReimbursementHolder WithOtherDeductions(IEnumerable<PayStubOtherDeduction> deductions);
    IReimbursementHolder WithNoMoreDeductions();
}

public interface IReimbursementHolder
{
    IPayVacations WithReimbursement(IReadOnlyCollection<PayStubItem> items);
    IPayVacations WithoutReimbursement();
}

public interface IPayVacations
{
    IPayStubBuilder PayVacations(bool pay = true);
}

public interface IPayStubBuilder
{
    Task<Result<PayStub>> Build();
}