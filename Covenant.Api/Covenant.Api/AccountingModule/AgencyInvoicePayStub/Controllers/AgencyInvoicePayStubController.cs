using Covenant.Api.AccountingModule.Shared;
using Covenant.Common.Repositories.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.AgencyInvoicePayStub.Controllers
{
    [Route(RouteName)]
    public class AgencyInvoicePayStubController : AccountingBaseController
    {
        public const string RouteName = "api/v4/Accounting/Invoice/{invoiceId}/PayStub";

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IPayStubRepository repository, Guid invoiceId) =>
            Ok(await repository.GetPayStubs(invoiceId));
    }
}