using Covenant.Api.AccountingModule.Shared;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.SkipPayrollNumber.Controllers
{
    [Route("api/v2/Accounting/SkipPayrollNumber")]
    public class AccountingSkipPayrollNumberV2Controller : AccountingBaseController
    {
        private readonly ISkipPayrollNumberRepository _repository;
        public AccountingSkipPayrollNumberV2Controller(ISkipPayrollNumberRepository repository) => _repository = repository;

        /// <summary>
        /// Gets the list of payroll numbers configured to be skipped.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<BaseModel<Guid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() => Ok(await _repository.Get(string.Empty));

        /// <summary>
        /// Creates a new skipped payroll number entry.
        /// </summary>
        /// <param name="model">Payroll number to skip.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] BaseModel<Guid> model)
        {
            if (model is null || !ModelState.IsValid) return BadRequest(ModelState);
            Result result = await _repository.Create(model);
            if (result) return Ok();
            return BadRequest(ModelState.AddErrors(result.Errors));
        }
    }
}
