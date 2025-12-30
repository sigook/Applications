using System.Threading.Tasks;
using Covenant.Deductions.Entities;
using Covenant.Deductions.Repositories;
using Microsoft.Extensions.Logging;

namespace Covenant.Infrastructure.Deductions
{
	public class ProvincialTaxTablesLoader : TaxTablesLoader
	{
		private readonly IDeductionsRepository _repository;

		public ProvincialTaxTablesLoader(IDeductionsRepository repository, ILogger<ProvincialTaxTablesLoader> logger) : base(logger) => _repository = repository;

		public async Task LoadWeeklyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year, 
				r => _repository.CreateProvincialTaxWeekly(new ProvincialTaxWeekly(r)),
				y => _repository.DeleteProvincialTaxWeekly(year), 
				() => _repository.SaveChangesAsync());

		public async Task LoadBiWeeklyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year,
				r => _repository.CreateProvincialTaxBiWeekly(new ProvincialTaxBiWeekly(r)),
				y => _repository.DeleteProvincialTaxBiWeekly(y),
				() => _repository.SaveChangesAsync());
		
		public async Task LoadSemiMonthlyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year,
				r => _repository.CreateProvincialTaxSemiMonthly(new ProvincialTaxSemiMonthly(r)),
				y => _repository.DeleteProvincialTaxSemiMonthly(y),
				() => _repository.SaveChangesAsync());
		
		public async Task LoadMonthlyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year,
				r => _repository.CreateProvincialTaxMonthly(new ProvincialTaxMonthly(r)),
				y => _repository.DeleteProvincialTaxMonthly(y),
				() => _repository.SaveChangesAsync());
	}
}