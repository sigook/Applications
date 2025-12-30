using Covenant.Common.Configuration;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Utils.Extensions;
using Covenant.Deductions.Models;
using Covenant.Deductions.Repositories;

namespace Covenant.Deductions.Services;

public class PayrollDeductionsAndContributionsCalculator : IPayrollDeductionsAndContributionsCalculator
{
    private readonly Rates _rates;
    private readonly IDeductionsRepository _deductionsRepository;

    public PayrollDeductionsAndContributionsCalculator(Rates rates, IDeductionsRepository deductionsRepository)
    {
        _rates = rates;
        _deductionsRepository = deductionsRepository;
    }

    public async Task<PayrollDeductionsAndContributionsResult> CalculateFor(decimal earnings, int numberOfWeeks, int year, WorkerProfileTaxCategory workerProfileTaxCategory)
    {
        decimal cpp;
        decimal provincialTax;
        decimal federalTax;
        var paymentPeriod = PaymentPeriodExtensions.From(numberOfWeeks);
        var provincialCategory = workerProfileTaxCategory?.ProvincialCategory ?? TaxCategory.Cc1;
        var federalCategory = workerProfileTaxCategory?.FederalCategory ?? TaxCategory.Cc1;
        switch (paymentPeriod)
        {
            case PaymentPeriod.Weekly:
                cpp = await _deductionsRepository.GetCppWeekly(earnings, year);
                provincialTax = (await _deductionsRepository.GetProvincialTaxWeekly(earnings, year, provincialCategory)).GetValueOrDefault();
                federalTax = (await _deductionsRepository.GetFederalTaxWeekly(earnings, year,  federalCategory)).GetValueOrDefault();
                break;
            case PaymentPeriod.Biweekly:
                cpp = await _deductionsRepository.GetCppBiWeekly(earnings, year);
                provincialTax = (await _deductionsRepository.GetProvincialTaxBiWeekly(earnings, year, provincialCategory)).GetValueOrDefault();
                federalTax = (await _deductionsRepository.GetFederalTaxBiWeekly(earnings, year, federalCategory)).GetValueOrDefault();
                break;
            case PaymentPeriod.SemiMonthly:
                cpp = await _deductionsRepository.GetCppSemiMonthly(earnings, year);
                provincialTax = (await _deductionsRepository.GetProvincialTaxSemiMonthly(earnings, year, provincialCategory)).GetValueOrDefault();
                federalTax = (await _deductionsRepository.GetFederalTaxSemiMonthly(earnings, year, federalCategory)).GetValueOrDefault();
                break;
            case PaymentPeriod.Monthly:
                cpp = await _deductionsRepository.GetCppMonthly(earnings, year);
                provincialTax = (await _deductionsRepository.GetProvincialTaxMonthly(earnings, year, provincialCategory)).GetValueOrDefault();
                federalTax = (await _deductionsRepository.GetFederalTaxMonthly(earnings, year, federalCategory)).GetValueOrDefault();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        cpp = workerProfileTaxCategory?.Cpp ?? cpp;
        var ei = workerProfileTaxCategory?.Ei ?? decimal.Multiply(earnings, _rates.EmploymentInsurance).DefaultMoneyRound();
        return new PayrollDeductionsAndContributionsResult(cpp, ei, federalTax, provincialTax);
    }
}

public enum PaymentPeriod
{
    Weekly = 52,
    Biweekly = 26,
    SemiMonthly = 24,
    Monthly = 12
}

public static class PaymentPeriodExtensions
{
    public static PaymentPeriod From(int weeks)
    {
        switch (weeks)
        {
            case 1: return PaymentPeriod.Weekly;
            case 2: return PaymentPeriod.Biweekly;
            case 3: return PaymentPeriod.SemiMonthly;
            case 4: return PaymentPeriod.Monthly;
            default: return PaymentPeriod.Monthly;
        }
    }
}