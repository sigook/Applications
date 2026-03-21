using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Functionals;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Worker;
using Covenant.Deductions.Services;
using Covenant.PayStubs.Models;
using Covenant.PayStubs.Utils;

namespace Covenant.PayStubs.Services;

[Obsolete("Use IPayStubService.CreateManualPayStub instead")]
public class CreatePayStubWithOutTimeSheet(Rates rates,
    IPayrollDeductionsAndContributionsCalculator deductionsAndContributionsCalculator,
    IPayStubRepository payStubRepository,
    IWorkerRepository workerRepository)
{
    public async Task<Result<PayStub>> Create(CreatePayStubModel model)
    {
        if (model.PayStubNumber <= 0)
        {
            var numbers = await payStubRepository.GetNextPayStubNumbers(1);
            model.PayStubNumber = numbers.First().NextNumber;
        }
        else
        {
            if (await payStubRepository.IsPayStubNumberTaken(model.PayStubNumber))
                return Result.Fail<PayStub>("Pay stub number is taken");
        }
        var items = model.ToPayStubItems();
        if (!items) return Result.Fail<PayStub>(items.Errors);
        var holidays = ToHolidays(model);
        if (!holidays) return Result.Fail<PayStub>(holidays.Errors);
        var otherDeductions = model.OtherDeductions > 0
            ? [PayStubOtherDeduction.CreateDefaultDeduction(model.OtherDeductions, model.OtherDeductionsDescription)]
            : Array.Empty<PayStubOtherDeduction>();
        var payStub = await PayStubBuilder.PayStub(rates, deductionsAndContributionsCalculator, workerRepository)
            .WithPayStubNumber(model.PayStubNumber)
            .WithWorkerProfileId(model.WorkerProfileId)
            .WithTypeOfWork(model.TypeOfWork)
            .WithWorkBeginning(model.WorkBegins)
            .WithWorkEnding(model.WorkEnd)
            .WithCreationDate(DateTime.Now)
            .WithItems(items.Value)
            .WithoutWageDetails()
            .WithPublicHolidaysToPay(holidays.Value)
            .WithOtherDeductions(otherDeductions)
            .WithoutReimbursement()
            .PayVacations(model.PayVacations)
            .Build();
        if (!payStub) return Result.Fail<PayStub>(payStub.Errors);
        await payStubRepository.Create(payStub.Value);
        await payStubRepository.SaveChangesAsync();
        return Result.Ok(payStub.Value);
    }

    private static Result<List<PayStubPublicHoliday>> ToHolidays(CreatePayStubModel model)
    {
        IEnumerable<Result<PayStubPublicHoliday>> results = [.. model.Holidays.Select(h => PayStubPublicHoliday.Create(h.Holiday, h.Amount, "This amount was added manually"))];
        IEnumerable<ResultError> resultErrors = [.. results.Where(r => r.IsFailure).SelectMany(r => r.Errors)];
        return resultErrors.Any()
            ? Result.Fail<List<PayStubPublicHoliday>>(resultErrors)
            : Result.Ok(results.Select(r => r.Value).ToList());
    }
}