using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook.Agency.Accounting;

[ApiController]
[Authorize]
[Route(RouteName)]
public class DeductionCppController(ICppDeductionImportService importService) : ControllerBase
{
    public const string RouteName = "api/Accounting/Deduction/Cpp";

    /// <summary>
    /// Imports a CPP deduction table from a CRA PDF already stored in the cra-tables container, keeping the last two years.
    /// </summary>
    /// <param name="model">Blob name, pay period and year of the table to import.</param>
    [HttpPost("Blob")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostBlob([FromBody] ImportCppFromBlobModel model)
    {
        var result = await importService.ImportFromBlob(model);
        if (!result)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
    }
}
