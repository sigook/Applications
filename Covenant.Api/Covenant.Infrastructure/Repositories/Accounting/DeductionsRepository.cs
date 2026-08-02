using EFCore.BulkExtensions;
using Covenant.Common.Models;
using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Covenant.Common.Utils.Extensions;
using Covenant.Common.Enums;

namespace Covenant.Infrastructure.Repositories.Accounting;

public class DeductionsRepository(CovenantContext context) : IDeductionsRepository
{
    public Task<decimal> GetCpp(decimal earnings, int year, PayPeriod payPeriod) =>
        context.CppDeductions
            .Where(c => c.Year == year && c.PayPeriod == payPeriod && earnings >= c.From && earnings <= c.To)
            .Select(w => w.Cpp)
            .SingleOrDefaultAsync();

    public async Task<int> ReplaceCpp(int year, PayPeriod payPeriod, IReadOnlyList<CppDeduction> rows)
    {
        if (!context.Database.IsRelational())
        {
            context.CppDeductions.RemoveRange(
                await context.CppDeductions.Where(w => w.Year == year && w.PayPeriod == payPeriod).ToListAsync());
            await context.CppDeductions.AddRangeAsync(rows);
            await context.SaveChangesAsync();
            return rows.Count;
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.CppDeductions.Where(w => w.Year == year && w.PayPeriod == payPeriod).ExecuteDeleteAsync();
        await context.BulkInsertAsync(rows.ToList());
        await transaction.CommitAsync();
        return rows.Count;
    }

    public Task<decimal?> GetTax(decimal earnings, int year, PayPeriod payPeriod, TaxType taxType, TaxCategory category) =>
        context.TaxDeductions
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

    public Task<PaginatedList<TaxModel>> GetTax(DeductionPagination pagination, PayPeriod payPeriod, TaxType taxType) =>
        context.TaxDeductions
            .Where(w => w.Year == pagination.Year && w.PayPeriod == payPeriod && w.TaxType == taxType)
            .Select(w => new TaxModel
            {
                Id = w.Id,
                PayPeriod = w.PayPeriod,
                TaxType = w.TaxType,
                From = w.From,
                To = w.To,
                Cc0 = w.Cc0,
                Cc1 = w.Cc1,
                Cc2 = w.Cc2,
                Cc3 = w.Cc3,
                Cc4 = w.Cc4,
                Cc5 = w.Cc5,
                Cc6 = w.Cc6,
                Cc7 = w.Cc7,
                Cc8 = w.Cc8,
                Cc9 = w.Cc9,
                Cc10 = w.Cc10,
                Year = w.Year
            })
            .OrderBy(o => o.From)
            .ToPaginatedList(pagination);

    public async Task CreateTax(TaxDeduction entity) => await context.TaxDeductions.AddAsync(entity);

    public async Task DeleteTax(int year, PayPeriod payPeriod, TaxType taxType) =>
        context.TaxDeductions.RemoveRange(
            await context.TaxDeductions.Where(w => w.Year == year && w.PayPeriod == payPeriod && w.TaxType == taxType).ToListAsync());

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
