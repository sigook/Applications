using System.Threading.Tasks;
using Covenant.Deductions.Entities;
using Covenant.Deductions.Repositories;
using Microsoft.Extensions.Logging;

namespace Covenant.Infrastructure.Deductions
{
	public class FederalTaxTablesLoader : TaxTablesLoader
	{
		private readonly IDeductionsRepository _repository;

		public FederalTaxTablesLoader(IDeductionsRepository repository, ILogger<FederalTaxTablesLoader> logger) : base(logger) => _repository = repository;

		public async Task LoadWeeklyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year, 
				r => _repository.CreateFederalTaxWeekly(new TaxWeekly(r)),
				y => _repository.DeleteFederalTaxWeekly(y), 
				() => _repository.SaveChangesAsync());

		public async Task LoadBiWeeklyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year,
				r => _repository.CreateFederalTaxBiWeekly(new FederalTaxBiWeekly(r)),
				y => _repository.DeleteFederalTaxBiWeekly(y),
				() => _repository.SaveChangesAsync());
		
		public async Task LoadSemiMonthlyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year,
				r => _repository.CreateFederalTaxSemiMonthly(new FederalTaxSemiMonthly(r)),
				y => _repository.DeleteFederalTaxSemiMonthly(y),
				() => _repository.SaveChangesAsync());
		
		public async Task LoadMonthlyTablesFromExcel(string excelPath, int year) =>
			await LoadTablesFromExcel(excelPath, year,
				r => _repository.CreateFederalTaxMonthly(new FederalTaxMonthly(r)),
				y => _repository.DeleteFederalTaxMonthly(y),
				() => _repository.SaveChangesAsync());
	}
}