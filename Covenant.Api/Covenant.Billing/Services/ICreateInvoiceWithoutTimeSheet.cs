using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;

namespace Covenant.Billing.Services
{
    public interface ICreateInvoiceWithoutTimeSheet
    {
        Task<Result<Invoice>> Preview(Guid companyProfileId, CreateInvoiceModel model);
        Task<Result<Invoice>> Create(Guid companyProfileId, CreateInvoiceModel model);
    }
}