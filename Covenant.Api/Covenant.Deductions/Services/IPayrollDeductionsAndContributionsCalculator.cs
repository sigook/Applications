using Covenant.Common.Entities.Worker;
using Covenant.Deductions.Models;

namespace Covenant.Deductions.Services;

public interface IPayrollDeductionsAndContributionsCalculator
{
    Task<PayrollDeductionsAndContributionsResult> CalculateFor(decimal earnings, int numberOfWeeks, int year, WorkerProfileTaxCategory workerProfileTaxCategory);
}