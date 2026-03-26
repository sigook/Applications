using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Worker;
using Covenant.Deductions.Services;

namespace Covenant.PayStubs.Services;

[Obsolete("Use IPayStubService.CreateManualPayStub instead")]
public class CreatePayStubWithOutTimeSheet(Rates rates,
    IPayrollDeductionsAndContributionsCalculator deductionsAndContributionsCalculator,
    IPayStubRepository payStubRepository,
    IWorkerRepository workerRepository)
{
    public async Task<Result<PayStub>> Create(CreatePayStubModel model)
    {
        var nextNumbers = await payStubRepository.GetNextPayStubNumbers(1);
        var payStubNumber = nextNumbers.First().NextNumber;

        var payStubItems = new List<PayStubItem>();
        var itemErrors = new List<ResultError>();

        foreach (var item in model.Items)
        {
            var result = item.Type switch
            {
                PayStubItemType.Regular => PayStubItem.CreateRegular(item.Quantity, item.UnitPrice),
                PayStubItemType.OtherRegular => PayStubItem.CreateOtherRegular(item.Quantity, item.UnitPrice),
                PayStubItemType.Overtime => PayStubItem.CreateOvertime(item.Quantity, item.UnitPrice),
                PayStubItemType.StatutoryHoliday => PayStubItem.CreateStatutoryHoliday(item.UnitPrice),
                PayStubItemType.StatutoryWorkedHoliday => PayStubItem.CreateStatutoryWorkedHoliday(item.Quantity, item.UnitPrice),
                PayStubItemType.NightShift => PayStubItem.CreateNightShift(item.Quantity, item.UnitPrice),
                PayStubItemType.Missing => PayStubItem.CreateMissing(item.Quantity, item.UnitPrice),
                PayStubItemType.MissingOvertime => PayStubItem.CreateMissingOvertime(item.Quantity, item.UnitPrice),
                PayStubItemType.Vacations => PayStubItem.CreateItem("Vacations", item.Quantity, item.UnitPrice, PayStubItemType.Vacations),
                PayStubItemType.Other => PayStubItem.CreateBonusOthersItem(item.Quantity, item.UnitPrice, null),
                PayStubItemType.Reimbursement => PayStubItem.CreateReimbursements(decimal.Multiply(new decimal(item.Quantity), item.UnitPrice), null),
                _ => Result.Fail<PayStubItem>($"Unknown item type: {item.Type}")
            };
            if (result) payStubItems.Add(result.Value);
            else itemErrors.AddRange(result.Errors);
        }

        if (itemErrors.Any()) return Result.Fail<PayStub>(itemErrors);

        var otherDeductions = model.OtherDeductions > 0
            ? [PayStubOtherDeduction.CreateDefaultDeduction(model.OtherDeductions, model.OtherDeductionsDescription)]
            : Array.Empty<PayStubOtherDeduction>();

        var payStub = await PayStubBuilder.PayStub(rates, deductionsAndContributionsCalculator, workerRepository)
            .WithPayStubNumber(payStubNumber)
            .WithWorkerProfileId(model.WorkerProfileId)
            .WithPosition(model.Position)
            .WithWorkBeginning(model.WorkBegins)
            .WithWorkEnding(model.WorkEnd)
            .WithCreationDate(DateTime.Now)
            .WithItems(payStubItems)
            .WithoutWageDetails()
            .WithPublicHolidaysToPay([])
            .WithOtherDeductions(otherDeductions)
            .WithoutReimbursement()
            .PayVacations(model.PayVacations)
            .Build();

        if (!payStub) return Result.Fail<PayStub>(payStub.Errors);
        await payStubRepository.Create(payStub.Value);
        await payStubRepository.SaveChangesAsync();
        return Result.Ok(payStub.Value);
    }
}
