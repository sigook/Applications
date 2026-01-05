using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.SkipPayrollNumber.Controllers
{
    [Route("api/v2/Accounting/SkipPayrollNumber")]
    public class AccountingSkipPayrollNumberV2Controller : AccountingBaseController
    {
        private readonly ISkipPayrollNumberRepository _repository;
        public AccountingSkipPayrollNumberV2Controller(ISkipPayrollNumberRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _repository.Get(string.Empty));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BaseModel<Guid> model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            Result result = await _repository.Create(model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}