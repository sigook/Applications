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
	public class DeductionFederalTaxController : DeductionControllerBase
	{
		public const string RouteName = "api/Accounting/Deduction/FederalTax";
		private readonly IDeductionsRepository _repository;

		public DeductionFederalTaxController(IDeductionsRepository repository) => _repository = repository;

		/// <summary>
		/// Gets the weekly federal tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("Weekly")]
		[ProducesResponseType(typeof(PaginatedList<TaxWeekly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetWeekly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxWeekly(pagination));

		/// <summary>
		/// Gets the bi-weekly federal tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("BiWeekly")]
		[ProducesResponseType(typeof(PaginatedList<FederalTaxBiWeekly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetBiWeekly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxBiWeekly(pagination));

		/// <summary>
		/// Gets the semi-monthly federal tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("SemiMonthly")]
		[ProducesResponseType(typeof(PaginatedList<FederalTaxSemiMonthly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetSemiMonthly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxSemiMonthly(pagination));

		/// <summary>
		/// Gets the monthly federal tax deduction table for the requested year.
		/// </summary>
		/// <param name="pagination">Year and paging parameters.</param>
		[HttpGet("Monthly")]
		[ProducesResponseType(typeof(PaginatedList<FederalTaxMonthly>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetMonthly(DeductionPagination pagination) => Ok(await _repository.GetFederalTaxMonthly(pagination));

		/// <summary>
		/// Loads the weekly federal tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Federal tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("Weekly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostWeeklyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadWeeklyTablesFromExcel(path, year));

		/// <summary>
		/// Loads the bi-weekly federal tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Federal tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("BiWeekly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostBiWeeklyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadBiWeeklyTablesFromExcel(path, year));

		/// <summary>
		/// Loads the semi-monthly federal tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Federal tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("SemiMonthly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostSemiMonthlyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadSemiMonthlyTablesFromExcel(path, year));

		/// <summary>
		/// Loads the monthly federal tax deduction table from an uploaded Excel file.
		/// </summary>
		/// <param name="loader">Federal tax deduction tables loader.</param>
		/// <param name="model">Excel file and target year.</param>
		[HttpPost("Monthly/Excel")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PostMonthlyExcel([FromServices] FederalTaxTablesLoader loader,
			[FromForm] CreateDeductionModel model) =>
			await Load(model, async (path, year) => await loader.LoadMonthlyTablesFromExcel(path, year));
	}
}
