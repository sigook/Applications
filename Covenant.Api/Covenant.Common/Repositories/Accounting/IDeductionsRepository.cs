using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Models.Accounting.Deductions;

namespace Covenant.Common.Repositories.Accounting;

public interface IDeductionsRepository
{
    Task<decimal> GetCpp(decimal earnings, int year, PayPeriod payPeriod);
    Task<int> ImportCpp(int year, PayPeriod payPeriod, IReadOnlyList<CppDeduction> rows, int yearsKept);

    Task<decimal?> GetTax(decimal earnings, int year, PayPeriod payPeriod, TaxType taxType, TaxCategory category);
    Task<PaginatedList<TaxModel>> GetTax(DeductionPagination pagination, PayPeriod payPeriod, TaxType taxType);
    Task CreateTax(TaxDeduction entity);
    Task DeleteTax(int year, PayPeriod payPeriod, TaxType taxType);

    Task SaveChangesAsync();
}
