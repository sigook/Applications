using Covenant.Common.Models;
using Covenant.Deductions.Entities;
using Covenant.Deductions.Models;
using Covenant.Deductions.Repositories;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Covenant.Common.Utils.Extensions;
using Covenant.Common.Enums;

namespace Covenant.Infrastructure.Deductions.Repositories
{
    public class DeductionsRepository : IDeductionsRepository
    {
        private readonly CovenantContext _context;
        public DeductionsRepository(CovenantContext context) => _context = context;

        public Task<decimal> GetCppWeekly(decimal earnings, int year) =>
            _context.CppWeekly.Where(c => c.Year == year && earnings >= c.From && earnings <= c.To) //This operator is different to TaxWeekly because the From and To is not the same
                .Select(w => w.Cpp).SingleOrDefaultAsync();

        public Task<decimal> GetCppBiWeekly(decimal earnings, int year) =>
            _context.CppBiWeekly.Where(c => c.Year == year && earnings >= c.From && earnings <= c.To) //This operator is different to TaxWeekly because the From and To is not the same
                .Select(w => w.Cpp).SingleOrDefaultAsync();

        public Task<decimal> GetCppSemiMonthly(decimal earnings, int year) =>
            _context.CppSemiMonthly.Where(c => c.Year == year && earnings >= c.From && earnings <= c.To) //This operator is different to TaxWeekly because the From and To is not the same
                .Select(w => w.Cpp).SingleOrDefaultAsync();

        public Task<decimal> GetCppMonthly(decimal earnings, int year) =>
            _context.CppMonthly.Where(c => c.Year == year && earnings >= c.From && earnings <= c.To) //This operator is different to TaxWeekly because the From and To is not the same
                .Select(w => w.Cpp).SingleOrDefaultAsync();

        public Task<PaginatedList<CppModel>> GetCppWeekly(DeductionPagination pagination) => _context.CppWeekly.Where(w => w.Year == pagination.Year)
            .Select(w => new CppModel { Id = w.Id, From = w.From, To = w.To, Cpp = w.Cpp, Year = w.Year })
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<CppModel>> GetCppBiWeekly(DeductionPagination pagination) => _context.CppBiWeekly.Where(w => w.Year == pagination.Year)
                .Select(w => new CppModel { Id = w.Id, From = w.From, To = w.To, Cpp = w.Cpp, Year = w.Year })
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<CppModel>> GetCppSemiMonthly(DeductionPagination pagination) => _context.CppSemiMonthly.Where(w => w.Year == pagination.Year)
                .Select(w => new CppModel { Id = w.Id, From = w.From, To = w.To, Cpp = w.Cpp, Year = w.Year })
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<CppModel>> GetCppMonthly(DeductionPagination pagination) => _context.CppMonthly.Where(w => w.Year == pagination.Year)
                .Select(w => new CppModel { Id = w.Id, From = w.From, To = w.To, Cpp = w.Cpp, Year = w.Year })
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<TaxWeekly>> GetFederalTaxWeekly(DeductionPagination pagination) =>
            _context.TaxWeekly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<FederalTaxBiWeekly>> GetFederalTaxBiWeekly(DeductionPagination pagination) =>
            _context.FederalTaxBiWeekly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<FederalTaxSemiMonthly>> GetFederalTaxSemiMonthly(DeductionPagination pagination) =>
            _context.FederalTaxSemiMonthly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<FederalTaxMonthly>> GetFederalTaxMonthly(DeductionPagination pagination) =>
            _context.FederalTaxMonthly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<ProvincialTaxWeekly>> GetProvincialTaxWeekly(DeductionPagination pagination) =>
            _context.ProvincialTaxWeekly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<ProvincialTaxBiWeekly>> GetProvincialTaxBiWeekly(DeductionPagination pagination) =>
            _context.ProvincialTaxBiWeekly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<ProvincialTaxSemiMonthly>> GetProvincialTaxSemiMonthly(DeductionPagination pagination) =>
            _context.ProvincialTaxSemiMonthly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public Task<PaginatedList<ProvincialTaxMonthly>> GetProvincialTaxMonthly(DeductionPagination pagination) =>
            _context.ProvincialTaxMonthly.Where(w => w.Year == pagination.Year)
                .Select(w => w)
                .OrderBy(o => o.From).ToPaginatedList(pagination);

        public async Task CreateCppWeekly(CppWeekly entity) => await _context.CppWeekly.AddAsync(entity);

        public async Task DeleteCppWeekly(int year) => _context.CppWeekly.RemoveRange(await _context.CppWeekly.Where(w => w.Year == year).ToListAsync());

        public async Task CreateCppBiWeekly(CppBiWeekly entity) => await _context.CppBiWeekly.AddAsync(entity);

        public async Task DeleteCppBiWeekly(int year) => _context.CppBiWeekly.RemoveRange(await _context.CppBiWeekly.Where(w => w.Year == year).ToListAsync());

        public async Task CreateCppSemiMonthly(CppSemiMonthly entity) => await _context.CppSemiMonthly.AddAsync(entity);

        public async Task DeleteCppSemiMonthly(int year) => _context.CppSemiMonthly.RemoveRange(await _context.CppSemiMonthly.Where(w => w.Year == year).ToListAsync());

        public async Task CreateCppMonthly(CppMonthly entity) => await _context.CppMonthly.AddAsync(entity);

        public async Task DeleteCppMonthly(int year) => _context.CppMonthly.RemoveRange(await _context.CppMonthly.Where(w => w.Year == year).ToListAsync());
        public async Task CreateFederalTaxWeekly(TaxWeekly entity) => await _context.TaxWeekly.AddAsync(entity);
        public async Task CreateFederalTaxBiWeekly(FederalTaxBiWeekly entity) => await _context.FederalTaxBiWeekly.AddAsync(entity);
        public async Task CreateFederalTaxSemiMonthly(FederalTaxSemiMonthly entity) => await _context.FederalTaxSemiMonthly.AddAsync(entity);
        public async Task CreateFederalTaxMonthly(FederalTaxMonthly entity) => await _context.FederalTaxMonthly.AddAsync(entity);

        public async Task CreateProvincialTaxWeekly(ProvincialTaxWeekly entity) => await _context.ProvincialTaxWeekly.AddAsync(entity);
        public async Task CreateProvincialTaxBiWeekly(ProvincialTaxBiWeekly entity) => await _context.ProvincialTaxBiWeekly.AddAsync(entity);
        public async Task CreateProvincialTaxSemiMonthly(ProvincialTaxSemiMonthly entity) => await _context.ProvincialTaxSemiMonthly.AddAsync(entity);
        public async Task CreateProvincialTaxMonthly(ProvincialTaxMonthly entity) => await _context.ProvincialTaxMonthly.AddAsync(entity);

        public async Task DeleteFederalTaxWeekly(int year) => _context.TaxWeekly.RemoveRange(await _context.TaxWeekly.Where(w => w.Year == year).ToListAsync());
        public async Task DeleteFederalTaxSemiMonthly(int year) => _context.FederalTaxSemiMonthly.RemoveRange(await _context.FederalTaxSemiMonthly.Where(w => w.Year == year).ToListAsync());
        public async Task DeleteFederalTaxMonthly(int year) => _context.FederalTaxMonthly.RemoveRange(await _context.FederalTaxMonthly.Where(w => w.Year == year).ToListAsync());
        public async Task DeleteFederalTaxBiWeekly(int year) => _context.FederalTaxBiWeekly.RemoveRange(await _context.FederalTaxBiWeekly.Where(w => w.Year == year).ToListAsync());
        public async Task DeleteProvincialTaxWeekly(int year) => _context.ProvincialTaxWeekly.RemoveRange(await _context.ProvincialTaxWeekly.Where(w => w.Year == year).ToListAsync());
        public async Task DeleteProvincialTaxBiWeekly(int year) => _context.ProvincialTaxBiWeekly.RemoveRange(await _context.ProvincialTaxBiWeekly.Where(w => w.Year == year).ToListAsync());
        public async Task DeleteProvincialTaxSemiMonthly(int year) => _context.ProvincialTaxSemiMonthly.RemoveRange(await _context.ProvincialTaxSemiMonthly.Where(w => w.Year == year).ToListAsync());
        public async Task DeleteProvincialTaxMonthly(int year) => _context.ProvincialTaxMonthly.RemoveRange(await _context.ProvincialTaxMonthly.Where(w => w.Year == year).ToListAsync());

        public Task<decimal?> GetProvincialTaxWeekly(decimal earnings, int year, TaxCategory provincialCategory)
        {
            var provincialTax = _context.ProvincialTaxWeekly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w => 
                    provincialCategory == TaxCategory.Cc0 ? w.Cc0 :
                    provincialCategory == TaxCategory.Cc1 ? w.Cc1 :
                    provincialCategory == TaxCategory.Cc2 ? w.Cc2 :
                    provincialCategory == TaxCategory.Cc3 ? w.Cc3 :
                    provincialCategory == TaxCategory.Cc4 ? w.Cc4 :
                    provincialCategory == TaxCategory.Cc5 ? w.Cc5 :
                    provincialCategory == TaxCategory.Cc6 ? w.Cc6 :
                    provincialCategory == TaxCategory.Cc7 ? w.Cc7 :
                    provincialCategory == TaxCategory.Cc8 ? w.Cc8 :
                    provincialCategory == TaxCategory.Cc9 ? w.Cc9 :
                    provincialCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return provincialTax;
        }

        public Task<decimal?> GetProvincialTaxBiWeekly(decimal earnings, int year, TaxCategory provincialCategory)
        {
            var provincialTax = _context.ProvincialTaxBiWeekly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w =>
                    provincialCategory == TaxCategory.Cc0 ? w.Cc0 :
                    provincialCategory == TaxCategory.Cc1 ? w.Cc1 :
                    provincialCategory == TaxCategory.Cc2 ? w.Cc2 :
                    provincialCategory == TaxCategory.Cc3 ? w.Cc3 :
                    provincialCategory == TaxCategory.Cc4 ? w.Cc4 :
                    provincialCategory == TaxCategory.Cc5 ? w.Cc5 :
                    provincialCategory == TaxCategory.Cc6 ? w.Cc6 :
                    provincialCategory == TaxCategory.Cc7 ? w.Cc7 :
                    provincialCategory == TaxCategory.Cc8 ? w.Cc8 :
                    provincialCategory == TaxCategory.Cc9 ? w.Cc9 :
                    provincialCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return provincialTax;
        }
            
        public Task<decimal?> GetProvincialTaxSemiMonthly(decimal earnings, int year, TaxCategory provincialCategory)
        {
            var provincialTax = _context.ProvincialTaxSemiMonthly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w =>
                    provincialCategory == TaxCategory.Cc0 ? w.Cc0 :
                    provincialCategory == TaxCategory.Cc1 ? w.Cc1 :
                    provincialCategory == TaxCategory.Cc2 ? w.Cc2 :
                    provincialCategory == TaxCategory.Cc3 ? w.Cc3 :
                    provincialCategory == TaxCategory.Cc4 ? w.Cc4 :
                    provincialCategory == TaxCategory.Cc5 ? w.Cc5 :
                    provincialCategory == TaxCategory.Cc6 ? w.Cc6 :
                    provincialCategory == TaxCategory.Cc7 ? w.Cc7 :
                    provincialCategory == TaxCategory.Cc8 ? w.Cc8 :
                    provincialCategory == TaxCategory.Cc9 ? w.Cc9 :
                    provincialCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return provincialTax;
        }
            
        public Task<decimal?> GetProvincialTaxMonthly(decimal earnings, int year, TaxCategory provincialCategory)
        {
            var provincialTax = _context.ProvincialTaxMonthly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w =>
                    provincialCategory == TaxCategory.Cc0 ? w.Cc0 :
                    provincialCategory == TaxCategory.Cc1 ? w.Cc1 :
                    provincialCategory == TaxCategory.Cc2 ? w.Cc2 :
                    provincialCategory == TaxCategory.Cc3 ? w.Cc3 :
                    provincialCategory == TaxCategory.Cc4 ? w.Cc4 :
                    provincialCategory == TaxCategory.Cc5 ? w.Cc5 :
                    provincialCategory == TaxCategory.Cc6 ? w.Cc6 :
                    provincialCategory == TaxCategory.Cc7 ? w.Cc7 :
                    provincialCategory == TaxCategory.Cc8 ? w.Cc8 :
                    provincialCategory == TaxCategory.Cc9 ? w.Cc9 :
                    provincialCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return provincialTax;
        }

        public Task<decimal?> GetFederalTaxWeekly(decimal earnings, int year, TaxCategory federalCategory)
        {
            var federalTax = _context.TaxWeekly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w =>
                    federalCategory == TaxCategory.Cc0 ? w.Cc0 :
                    federalCategory == TaxCategory.Cc1 ? w.Cc1 :
                    federalCategory == TaxCategory.Cc2 ? w.Cc2 :
                    federalCategory == TaxCategory.Cc3 ? w.Cc3 :
                    federalCategory == TaxCategory.Cc4 ? w.Cc4 :
                    federalCategory == TaxCategory.Cc5 ? w.Cc5 :
                    federalCategory == TaxCategory.Cc6 ? w.Cc6 :
                    federalCategory == TaxCategory.Cc7 ? w.Cc7 :
                    federalCategory == TaxCategory.Cc8 ? w.Cc8 :
                    federalCategory == TaxCategory.Cc9 ? w.Cc9 :
                    federalCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return federalTax;
        }
            
        public Task<decimal?> GetFederalTaxBiWeekly(decimal earnings, int year, TaxCategory federalCategory)
        {
            var federalTax = _context.FederalTaxBiWeekly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w =>
                    federalCategory == TaxCategory.Cc0 ? w.Cc0 :
                    federalCategory == TaxCategory.Cc1 ? w.Cc1 :
                    federalCategory == TaxCategory.Cc2 ? w.Cc2 :
                    federalCategory == TaxCategory.Cc3 ? w.Cc3 :
                    federalCategory == TaxCategory.Cc4 ? w.Cc4 :
                    federalCategory == TaxCategory.Cc5 ? w.Cc5 :
                    federalCategory == TaxCategory.Cc6 ? w.Cc6 :
                    federalCategory == TaxCategory.Cc7 ? w.Cc7 :
                    federalCategory == TaxCategory.Cc8 ? w.Cc8 :
                    federalCategory == TaxCategory.Cc9 ? w.Cc9 :
                    federalCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return federalTax;
        }
            
        public Task<decimal?> GetFederalTaxSemiMonthly(decimal earnings, int year, TaxCategory federalCategory)
        {
            var federalTax = _context.FederalTaxSemiMonthly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w =>
                    federalCategory == TaxCategory.Cc0 ? w.Cc0 :
                    federalCategory == TaxCategory.Cc1 ? w.Cc1 :
                    federalCategory == TaxCategory.Cc2 ? w.Cc2 :
                    federalCategory == TaxCategory.Cc3 ? w.Cc3 :
                    federalCategory == TaxCategory.Cc4 ? w.Cc4 :
                    federalCategory == TaxCategory.Cc5 ? w.Cc5 :
                    federalCategory == TaxCategory.Cc6 ? w.Cc6 :
                    federalCategory == TaxCategory.Cc7 ? w.Cc7 :
                    federalCategory == TaxCategory.Cc8 ? w.Cc8 :
                    federalCategory == TaxCategory.Cc9 ? w.Cc9 :
                    federalCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return federalTax;
        }
            
        public Task<decimal?> GetFederalTaxMonthly(decimal earnings, int year, TaxCategory federalCategory)
        {
            var federalTax = _context.FederalTaxMonthly
                .Where(c => c.Year == year && earnings >= c.From && earnings < c.To)
                .Select(w =>
                    federalCategory == TaxCategory.Cc0 ? w.Cc0 :
                    federalCategory == TaxCategory.Cc1 ? w.Cc1 :
                    federalCategory == TaxCategory.Cc2 ? w.Cc2 :
                    federalCategory == TaxCategory.Cc3 ? w.Cc3 :
                    federalCategory == TaxCategory.Cc4 ? w.Cc4 :
                    federalCategory == TaxCategory.Cc5 ? w.Cc5 :
                    federalCategory == TaxCategory.Cc6 ? w.Cc6 :
                    federalCategory == TaxCategory.Cc7 ? w.Cc7 :
                    federalCategory == TaxCategory.Cc8 ? w.Cc8 :
                    federalCategory == TaxCategory.Cc9 ? w.Cc9 :
                    federalCategory == TaxCategory.Cc10 ? w.Cc10 : w.Cc1)
                .SingleOrDefaultAsync();
            return federalTax;
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}