using Covenant.Common.Entities.Deductions;
using Covenant.Common.Models;
using Covenant.Common.Models.Deductions;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Deductions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.Deduction.Tax
{
	[Route(RouteName)]
	public class DeductionProvincialTaxController : DeductionControllerBase
	{
		public const string RouteName = "api/Accounting/Deduction/ProvincialTax";
		private readonly IDeductionsRepository _repository;

		public DeductionProvincialTaxController(IDeductionsRepository repository) => _repository = repository;

		/// <summary>
		/// Gets the weekly provincial tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("Weekly")]
		[ProducesResponseType(typeof(PaginatedList<ProvincialTaxWeekly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetWeekly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxWeekly(pagination));

		/// <summary>
		/// Gets the bi-weekly provincial tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("BiWeekly")]
		[ProducesResponseType(typeof(PaginatedList<ProvincialTaxBiWeekly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetBiWeekly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxBiWeekly(pagination));

		/// <summary>
		/// Gets the semi-monthly provincial tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("SemiMonthly")]
		[ProducesResponseType(typeof(PaginatedList<ProvincialTaxSemiMonthly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetSemiMonthly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxSemiMonthly(pagination));

		/// <summary>
		/// Gets the monthly provincial tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("Monthly")]
		[ProducesResponseType(typeof(PaginatedList<ProvincialTaxMonthly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetMonthly(DeductionPagination pagination) => Ok(await _repository.GetProvincialTaxMonthly(pagination));

		/// <summary>
		/// Loads the weekly provincial tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Provincial tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("Weekly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostWeeklyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadWeeklyTablesFromExcel(path, year));

		/// <summary>
		/// Loads the bi-weekly provincial tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Provincial tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("BiWeekly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostBiWeeklyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadBiWeeklyTablesFromExcel(path, year));

		/// <summary>
		/// Loads the semi-monthly provincial tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Provincial tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("SemiMonthly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostSemiMonthlyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadSemiMonthlyTablesFromExcel(path, year));

		/// <summary>
		/// Loads the monthly provincial tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Provincial tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("Monthly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostMonthlyExcel([FromServices] ProvincialTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadMonthlyTablesFromExcel(path, year));
	}
}
