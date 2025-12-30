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

public class PayStubBuilder :
    IPayStubNumberHolder,
    IWorkerProfileIdHolder,
    ITypeOfWorkHolder,
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
    private string _typeOfWork;
    private DateTime _workBegins;
    private DateTime _workEnd;
    private DateTime _createdAt = DateTime.Now;
    private IEnumerable<PayStubWageDetail> _wageDetails = new List<PayStubWageDetail>();
    private IReadOnlyCollection<PayStubPublicHoliday> _holidaysToPay = Array.Empty<PayStubPublicHoliday>();
    private IReadOnlyCollection<PayStubItem> _items = new List<PayStubItem>();
    private bool _payVacations;
    private IEnumerable<PayStubOtherDeduction> _otherDeductions = Array.Empty<PayStubOtherDeduction>();
    private IReadOnlyCollection<PayStubItem> _reimbursements = new List<PayStubItem>();

    private PayStubBuilder(Rates rates, IPayrollDeductionsAndContributionsCalculator deductionsCalculator, IWorkerRepository workerRepository)
    {
        _rates = rates;
        _deductionsCalculator = deductionsCalculator;
        this.workerRepository = workerRepository;
    }

    public static IPayStubNumberHolder PayStub(Rates rates, IPayrollDeductionsAndContributionsCalculator deductionsAndContributionsCalculator, IWorkerRepository workerRepository) =>
        new PayStubBuilder(rates, deductionsAndContributionsCalculator, workerRepository);

    public IWorkerProfileIdHolder WithPayStubNumber(long number) => this.Chain(b => b._number = number);

    public ITypeOfWorkHolder WithWorkerProfileId(Guid workerProfileId) => this.Chain(b => b._workerProfileId = workerProfileId);

    public IDateWorkBeginHolder WithTypeOfWork(string type) => this.Chain(b => b._typeOfWork = type);

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

    public IWageDetailsHolder WithItems(IReadOnlyCollection<PayStubItem> items) => this.Chain(b => b._items = new List<PayStubItem>(items));

    public IPublicHolidaysToPayHolder WithWageDetails(IEnumerable<PayStubWageDetail> wageDetails) => this.Chain(b => b._wageDetails = wageDetails);

    public IPublicHolidaysToPayHolder WithoutWageDetails() => this;

    public IOtherDeductionsHolder WithPublicHolidaysToPay(IReadOnlyCollection<PayStubPublicHoliday> publicHolidays) =>
        this.Chain(b => b._holidaysToPay = new List<PayStubPublicHoliday>(publicHolidays ?? Array.Empty<PayStubPublicHoliday>()));

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
        if (!_items.Any()) return Result.Fail<PayStub>(ApiResources.There_is_not_enough_information_to_generate_pay_stub);
        if (_workBegins > _workEnd) return Result.Fail<PayStub>("Dates of work: start must be before end");
        var workerProfileTaxCategory = await workerRepository.GetWorkerProfileTaxCategory(_workerProfileId);
        var grossPayment = _items.Sum(i => i.Total).DefaultMoneyRound();
        var vacations = _payVacations ? PayrollFormulas.Vacations(grossPayment, _rates.Vacations).DefaultMoneyRound() : decimal.Zero;
        var publicHolidayPay = _holidaysToPay.Sum(r => r.Amount).DefaultMoneyRound();
        var totalEarnings = grossPayment.Add(vacations).Add(publicHolidayPay).DefaultMoneyRound();
        var numberOfWeeks = DateExtensions.GetNumberOfWeeksIn(_workBegins, _workEnd);
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
        var payStub = new PayStub(
            _workerProfileId,
            Common.Entities.Accounting.PayStub.PayStub.PayrollNumber(_number, _createdAt),
            _number,
            _typeOfWork,
            _workBegins,
            _workEnd,
            paymentDate,
            regularWage,
            grossPayment,
            vacations,
            publicHolidayPay,
            totalEarnings,
            payrollDeductionsAndContributions.Cpp,
            payrollDeductionsAndContributions.Ei,
            payrollDeductionsAndContributions.FederalTax,
            payrollDeductionsAndContributions.ProvincialTax,
            totalDeductions,
            totalPaid,
            _createdAt)
        {
            WeekEnding = _workEnd.GetWeekEndingCurrentWeek()
        };
        payStub.AddItems(_items.Concat(_reimbursements));
        payStub.AddWageDetails(_wageDetails);
        payStub.AddHolidays(_holidaysToPay);
        payStub.AddOtherDeductionsDetail(_otherDeductions);
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
    ITypeOfWorkHolder WithWorkerProfileId(Guid workerProfileId);
}

public interface ITypeOfWorkHolder
{
    IDateWorkBeginHolder WithTypeOfWork(string type);
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