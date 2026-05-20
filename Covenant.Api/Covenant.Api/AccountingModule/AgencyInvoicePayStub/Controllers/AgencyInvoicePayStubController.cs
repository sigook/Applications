using Covenant.Api.AccountingModule.Shared;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.AccountingModule.AgencyInvoicePayStub.Controllers
{
    [Route(RouteName)]
    public class AgencyInvoicePayStubController : AccountingBaseController
    {
        public const string RouteName = "api/v4/Accounting/Invoice/{invoiceId}/PayStub";

        /// <summary>
        /// Gets the pay stubs linked to an invoice, including delete-warning information.
        /// </summary>
        /// <param name="repository">Pay stub repository.</param>
        /// <param name="invoiceId">Identifier of the invoice.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<PayStubDeleteWarningListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromServices] IPayStubRepository repository, Guid invoiceId) =>
            Ok(await repository.GetPayStubs(invoiceId));
    }
}
