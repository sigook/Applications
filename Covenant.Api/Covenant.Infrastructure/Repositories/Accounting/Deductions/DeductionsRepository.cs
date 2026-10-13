using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Repositories.Accounting.Deductions;
using Covenant.Infrastructure.Contexts;
using Covenant.Common.Enums;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Accounting.Deductions;

public class DeductionsRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), IDeductionsRepository
{
    public Task<decimal> GetCpp(decimal earnings, int year, PayPeriod payPeriod) =>
        Context.CppDeductions
            .Where(c => c.Year == year && c.PayPeriod == payPeriod && earnings >= c.From && earnings <= c.To)
            .Select(w => w.Cpp)
            .SingleOrDefaultAsync();

    public Task<int> ImportCpp(int year, PayPeriod payPeriod, IReadOnlyList<CppDeduction> rows, int yearsKept)
    {
        var oldestYearKept = year - yearsKept + 1;
        return Import(rows, w => w.PayPeriod == payPeriod && (w.Year == year || w.Year < oldestYearKept));
    }

    public Task<decimal?> GetTax(decimal earnings, int year, PayPeriod payPeriod, TaxType taxType, TaxCategory category) =>
        Context.TaxDeductions
            .Where(c => c.Year == year && c.PayPeriod == payPeriod && c.TaxType == taxType &&
                        earnings >= c.From && earnings < c.To)
            .Select(w =>
                category == TaxCategory.Cc0 ? w.Cc0 :
                category == TaxCategory.Cc1 ? w.Cc1 :
                category == TaxCategory.Cc2 ? w.Cc2 :
                category == TaxCategory.Cc3 ? w.Cc3 :
                category == TaxCategory.Cc4 ? w.Cc4 :
                category == TaxCategory.Cc5 ? w.Cc5 :
                category == TaxCategory.Cc6 ? w.Cc6 :
                category == TaxCategory.Cc7 ? w.Cc7 :
                category == TaxCategory.Cc8 ? w.Cc8 :
                category == TaxCategory.Cc9 ? w.Cc9 :
                category == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
            .SingleOrDefaultAsync();

    public Task<int> ImportTax(int year, PayPeriod payPeriod, IReadOnlyList<TaxDeduction> rows, int yearsKept)
    {
        var oldestYearKept = year - yearsKept + 1;
        return Import(rows, w => w.PayPeriod == payPeriod && (w.Year == year || w.Year < oldestYearKept));
    }

    private async Task<int> Import<TDeduction>(IReadOnlyList<TDeduction> rows, Expression<Func<TDeduction, bool>> replaced)
        where TDeduction : class
    {
        if (!Context.Database.IsRelational())
        {
            Context.Set<TDeduction>().RemoveRange(await Context.Set<TDeduction>().Where(replaced).ToListAsync());
            await Context.Set<TDeduction>().AddRangeAsync(rows);
            await Context.SaveChangesAsync();
            return rows.Count;
        }

        await using var transaction = await Context.Database.BeginTransactionAsync();
        await Context.Set<TDeduction>().Where(replaced).ExecuteDeleteAsync();
        await Context.BulkInsertAsync(rows.ToList());
        await transaction.CommitAsync();
        return rows.Count;
    }
}
