using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[ApiController]
[Authorize]
[Route(RouteName)]
public class DeductionsController(IDeductionImportService importService) : ControllerBase
{
    public const string RouteName = "api/Accounting/Deduction";

    /// <summary>
    /// Imports a CPP deduction table from a CRA PDF already stored in the cra-tables container, keeping the last two years.
    /// </summary>
    /// <param name="model">Blob name, pay period and year of the table to import.</param>
    [HttpPost("Cpp/Blob")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostCppBlob([FromBody] ImportCraTableFromBlobModel model) =>
        Import(await importService.ImportCppFromBlob(model));

    /// <summary>
    /// Imports the federal and provincial income tax tables from a CRA PDF already stored in the cra-tables container,
    /// keeping the last two years.
    /// </summary>
    /// <param name="model">Blob name, pay period and year of the table to import.</param>
    [HttpPost("Tax/Blob")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostTaxBlob([FromBody] ImportCraTableFromBlobModel model) =>
        Import(await importService.ImportTaxFromBlob(model));

    private IActionResult Import(Result<int> result) =>
        result ? Ok(result.Value) : BadRequest(result.Errors);
}
