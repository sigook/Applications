using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Deductions.Entities;
using Covenant.Deductions.Models;

namespace Covenant.Deductions.Repositories
{
    public interface IDeductionsRepository
    {
        Task<decimal> GetCppWeekly(decimal earnings, int year);
        Task<PaginatedList<CppModel>> GetCppWeekly(DeductionPagination pagination);
        Task<decimal> GetCppBiWeekly(decimal earnings, int year);
        Task<PaginatedList<CppModel>> GetCppBiWeekly(DeductionPagination pagination);
        Task<decimal> GetCppSemiMonthly(decimal earnings, int year);
        Task<PaginatedList<CppModel>> GetCppSemiMonthly(DeductionPagination pagination);
        Task<decimal> GetCppMonthly(decimal earnings, int year);
        Task<PaginatedList<CppModel>> GetCppMonthly(DeductionPagination pagination);
        Task CreateCppWeekly(CppWeekly entity);
        Task DeleteCppWeekly(int year);
        Task CreateCppBiWeekly(CppBiWeekly entity);
        Task DeleteCppBiWeekly(int year);
        Task CreateCppSemiMonthly(CppSemiMonthly entity);
        Task DeleteCppSemiMonthly(int year);
        Task CreateCppMonthly(CppMonthly entity);
        Task DeleteCppMonthly(int year);

        Task CreateFederalTaxWeekly(TaxWeekly entity);
        Task CreateFederalTaxBiWeekly(FederalTaxBiWeekly entity);
        Task CreateFederalTaxSemiMonthly(FederalTaxSemiMonthly entity);
        Task CreateFederalTaxMonthly(FederalTaxMonthly entity);
        Task CreateProvincialTaxWeekly(ProvincialTaxWeekly entity);
        Task CreateProvincialTaxBiWeekly(ProvincialTaxBiWeekly entity);
        Task CreateProvincialTaxSemiMonthly(ProvincialTaxSemiMonthly entity);
        Task CreateProvincialTaxMonthly(ProvincialTaxMonthly entity);
        Task DeleteFederalTaxWeekly(int year);
        Task DeleteFederalTaxBiWeekly(int year);
        Task DeleteFederalTaxSemiMonthly(int year);
        Task DeleteFederalTaxMonthly(int year);
        Task DeleteProvincialTaxWeekly(int year);
        Task DeleteProvincialTaxBiWeekly(int year);
        Task DeleteProvincialTaxSemiMonthly(int year);
        Task DeleteProvincialTaxMonthly(int year);

        Task<PaginatedList<TaxWeekly>> GetFederalTaxWeekly(DeductionPagination pagination);
        Task<PaginatedList<FederalTaxBiWeekly>> GetFederalTaxBiWeekly(DeductionPagination pagination);
        Task<PaginatedList<FederalTaxSemiMonthly>> GetFederalTaxSemiMonthly(DeductionPagination pagination);
        Task<PaginatedList<FederalTaxMonthly>> GetFederalTaxMonthly(DeductionPagination pagination);
        Task<PaginatedList<ProvincialTaxWeekly>> GetProvincialTaxWeekly(DeductionPagination pagination);
        Task<PaginatedList<ProvincialTaxBiWeekly>> GetProvincialTaxBiWeekly(DeductionPagination pagination);
        Task<PaginatedList<ProvincialTaxSemiMonthly>> GetProvincialTaxSemiMonthly(DeductionPagination pagination);
        Task<PaginatedList<ProvincialTaxMonthly>> GetProvincialTaxMonthly(DeductionPagination pagination);

        Task<decimal?> GetProvincialTaxWeekly(decimal earnings, int year, TaxCategory provincialCategory);
        Task<decimal?> GetProvincialTaxBiWeekly(decimal earnings, int year, TaxCategory provincialCategory);
        Task<decimal?> GetProvincialTaxSemiMonthly(decimal earnings, int year, TaxCategory provincialCategory);
        Task<decimal?> GetProvincialTaxMonthly(decimal earnings, int year, TaxCategory provincialCategory);
        Task<decimal?> GetFederalTaxWeekly(decimal earnings, int year, TaxCategory federalCategory);
        Task<decimal?> GetFederalTaxBiWeekly(decimal earnings, int year, TaxCategory federalCategory);
        Task<decimal?> GetFederalTaxSemiMonthly(decimal earnings, int year, TaxCategory federalCategory);
        Task<decimal?> GetFederalTaxMonthly(decimal earnings, int year, TaxCategory federalCategory);
        Task SaveChangesAsync();
    }
}