using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Deductions;

namespace Covenant.Deductions.Services;

public interface IPayrollDeductionsAndContributionsCalculator
{
    Task<PayrollDeductionsAndContributionsResult> CalculateFor(decimal earnings, int numberOfWeeks, int year, WorkerProfileTaxCategory workerProfileTaxCategory);
}