using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Deductions;
using Covenant.Common.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.Deduction.Cpp
{
    [Route(RouteName)]
    public class DeductionCppController : DeductionControllerBase
    {
        public const string RouteName = "api/Accounting/Deduction/Cpp";
        private readonly IDeductionsRepository _repository;

        public DeductionCppController(IDeductionsRepository repository) => _repository = repository;

        /// <summary>
        /// Gets the weekly CPP deduction table for the requested year.
        /// </summary>
        /// <param name="pagination">Year and paging parameters.</param>
        [HttpGet("Weekly")]
        [ProducesResponseType(typeof(PaginatedList<CppModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWeekly(DeductionPagination pagination) => Ok(await _repository.GetCppWeekly(pagination));

        /// <summary>
        /// Gets the bi-weekly CPP deduction table for the requested year.
        /// </summary>
        /// <param name="pagination">Year and paging parameters.</param>
        [HttpGet("BiWeekly")]
        [ProducesResponseType(typeof(PaginatedList<CppModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBiWeekly(DeductionPagination pagination) => Ok(await _repository.GetCppBiWeekly(pagination));

        /// <summary>
        /// Gets the semi-monthly CPP deduction table for the requested year.
        /// </summary>
        /// <param name="pagination">Year and paging parameters.</param>
        [HttpGet("SemiMonthly")]
        [ProducesResponseType(typeof(PaginatedList<CppModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSemiMonthly(DeductionPagination pagination) => Ok(await _repository.GetCppSemiMonthly(pagination));

        /// <summary>
        /// Gets the monthly CPP deduction table for the requested year.
        /// </summary>
        /// <param name="pagination">Year and paging parameters.</param>
        [HttpGet("Monthly")]
        [ProducesResponseType(typeof(PaginatedList<CppModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthly(DeductionPagination pagination) => Ok(await _repository.GetCppMonthly(pagination));

        /// <summary>
        /// Loads the weekly CPP deduction table from an uploaded Excel file.
        /// </summary>
        /// <param name="loader">CPP deduction tables loader.</param>
        /// <param name="model">Excel file and target year.</param>
        [HttpPost("Weekly/Excel")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostWeeklyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadWeeklyTablesFromExcel(path, year));

        /// <summary>
        /// Loads the bi-weekly CPP deduction table from an uploaded Excel file.
        /// </summary>
        /// <param name="loader">CPP deduction tables loader.</param>
        /// <param name="model">Excel file and target year.</param>
        [HttpPost("BiWeekly/Excel")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostBiWeeklyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadBiWeeklyTablesFromExcel(path, year));

        /// <summary>
        /// Loads the semi-monthly CPP deduction table from an uploaded Excel file.
        /// </summary>
        /// <param name="loader">CPP deduction tables loader.</param>
        /// <param name="model">Excel file and target year.</param>
        [HttpPost("SemiMonthly/Excel")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SemiMonthlyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadSemiMonthlyTablesFromExcel(path, year));

        /// <summary>
        /// Loads the monthly CPP deduction table from an uploaded Excel file.
        /// </summary>
        /// <param name="loader">CPP deduction tables loader.</param>
        /// <param name="model">Excel file and target year.</param>
        [HttpPost("Monthly/Excel")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MonthlyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadMonthlyTablesFromExcel(path, year));
    }
}
