using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.Deduction
{
    [ApiController]
    [AllowAnonymous]
    public abstract class DeductionControllerBase : ControllerBase
    {
        protected async Task<IActionResult> Load(CreateDeductionModel model, Func<string, int, Task> load)
        {
            const string excelExtension = ".xlsx";
            if (!ModelState.IsValid || model.File is null ||
                !model.File.FileName.EndsWith(excelExtension, StringComparison.InvariantCultureIgnoreCase)) return BadRequest(ModelState);
            string path = Path.Combine(Path.GetTempPath(), $"{Path.GetTempFileName()}{excelExtension}");
            using (var stream = new FileStream(path, FileMode.Create)) await model.File.CopyToAsync(stream);
            await load(path, model.Year);
            return Ok();
        }
    }
}