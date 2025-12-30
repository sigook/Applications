using Covenant.Deductions.Models;
using Covenant.Deductions.Repositories;
using Covenant.Deductions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.Deduction.Cpp
{
    [Route(RouteName)]
    public class DeductionCppController : DeductionControllerBase
    {
        public const string RouteName = "api/Accounting/Deduction/Cpp";
        private readonly IDeductionsRepository _repository;

        public DeductionCppController(IDeductionsRepository repository) => _repository = repository;

        [HttpGet("Weekly")]
        public async Task<IActionResult> GetWeekly(DeductionPagination pagination) => Ok(await _repository.GetCppWeekly(pagination));

        [HttpGet("BiWeekly")]
        public async Task<IActionResult> GetBiWeekly(DeductionPagination pagination) => Ok(await _repository.GetCppBiWeekly(pagination));

        [HttpGet("SemiMonthly")]
        public async Task<IActionResult> GetSemiMonthly(DeductionPagination pagination) => Ok(await _repository.GetCppSemiMonthly(pagination));

        [HttpGet("Monthly")]
        public async Task<IActionResult> GetMonthly(DeductionPagination pagination) => Ok(await _repository.GetCppMonthly(pagination));

        [HttpPost("Weekly/Excel")]
        public async Task<IActionResult> PostWeeklyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadWeeklyTablesFromExcel(path, year));

        [HttpPost("BiWeekly/Excel")]
        public async Task<IActionResult> PostBiWeeklyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadBiWeeklyTablesFromExcel(path, year));

        [HttpPost("SemiMonthly/Excel")]
        public async Task<IActionResult> SemiMonthlyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadSemiMonthlyTablesFromExcel(path, year));

        [HttpPost("Monthly/Excel")]
        public async Task<IActionResult> MonthlyExcel([FromServices] ICppTablesLoader loader, [FromForm] CreateDeductionModel model) =>
            await Load(model, async (path, year) => await loader.LoadMonthlyTablesFromExcel(path, year));
    }
}