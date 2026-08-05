using Covenant.Common.Enums;
using Covenant.Common.Entities.Accounting.Deductions;

namespace Covenant.Common.Repositories.Accounting;

public interface IDeductionsRepository
{
    Task<decimal> GetCpp(decimal earnings, int year, PayPeriod payPeriod);
    Task<int> ImportCpp(int year, PayPeriod payPeriod, IReadOnlyList<CppDeduction> rows, int yearsKept);

    Task<decimal?> GetTax(decimal earnings, int year, PayPeriod payPeriod, TaxType taxType, TaxCategory category);
    Task<int> ImportTax(int year, PayPeriod payPeriod, IReadOnlyList<TaxDeduction> rows, int yearsKept);
}
