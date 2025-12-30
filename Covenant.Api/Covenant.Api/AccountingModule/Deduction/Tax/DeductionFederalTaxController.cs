using Covenant.Deductions.Models;
using Covenant.Deductions.Repositories;
using Covenant.Infrastructure.Deductions;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.Deduction.Tax
{
	[Route(RouteName)]
	public class DeductionFederalTaxController : DeductionControllerBase
	{
		public const string RouteName = "api/Accounting/Deduction/FederalTax";
		private readonly IDeductionsRepository _repository;

		public DeductionFederalTaxController(IDeductionsRepository repository) => _repository = repository;

		[HttpGet("Weekly")]
		public async Task<IActionResult> GetWeekly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxWeekly(pagination));

		[HttpGet("BiWeekly")]
		public async Task<IActionResult> GetBiWeekly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxBiWeekly(pagination));

		[HttpGet("SemiMonthly")]
		public async Task<IActionResult> GetSemiMonthly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxSemiMonthly(pagination));

		[HttpGet("Monthly")]
		public async Task<IActionResult> GetMonthly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxMonthly(pagination));

		[HttpPost("Weekly/Excel")]
		public async Task<IActionResult> PostWeeklyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadWeeklyTablesFromExcel(path, year));

		[HttpPost("BiWeekly/Excel")]
		public async Task<IActionResult> PostBiWeeklyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadBiWeeklyTablesFromExcel(path, year));

		[HttpPost("SemiMonthly/Excel")]
		public async Task<IActionResult> PostSemiMonthlyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadSemiMonthlyTablesFromExcel(path, year));

		[HttpPost("Monthly/Excel")]
		public async Task<IActionResult> PostMonthlyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadMonthlyTablesFromExcel(path, year));
	}
}