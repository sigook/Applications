using Covenant.Deductions.Models;
using Covenant.Deductions.Repositories;
using Covenant.Infrastructure.Deductions;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.Deduction.Tax
{
	[Route(RouteName)]
	public class DeductionProvincialTaxController : DeductionControllerBase
	{
		public const string RouteName = "api/Accounting/Deduction/ProvincialTax";
		private readonly IDeductionsRepository _repository;

		public DeductionProvincialTaxController(IDeductionsRepository repository) => _repository = repository;

		[HttpGet("Weekly")]
		public async Task<IActionResult> GetWeekly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxWeekly(pagination));

		[HttpGet("BiWeekly")]
		public async Task<IActionResult> GetBiWeekly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxBiWeekly(pagination));

		[HttpGet("SemiMonthly")]
		public async Task<IActionResult> GetSemiMonthly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxSemiMonthly(pagination));

		[HttpGet("Monthly")]
		public async Task<IActionResult> GetMonthly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxMonthly(pagination));

		[HttpPost("Weekly/Excel")]
		public async Task<IActionResult> PostWeeklyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadWeeklyTablesFromExcel(path, year));

		[HttpPost("BiWeekly/Excel")]
		public async Task<IActionResult> PostBiWeeklyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadBiWeeklyTablesFromExcel(path, year));

		[HttpPost("SemiMonthly/Excel")]
		public async Task<IActionResult> PostSemiMonthlyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadSemiMonthlyTablesFromExcel(path, year));

		[HttpPost("Monthly/Excel")]
		public async Task<IActionResult> PostMonthlyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadMonthlyTablesFromExcel(path, year));
	}
}