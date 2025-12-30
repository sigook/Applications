using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;

namespace Covenant.Billing.Services
{
    public interface ICreateInvoiceUsingTimeSheet
    {
        Task<Result<Invoice>> Create(Guid agencyId, Guid companyProfileId, IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model);
        Task<Result<Invoice>> Preview(Guid agencyId, Guid companyProfileId, IReadOnlyCollection<TimeSheetApprovedBillingModel> list, CreateInvoiceModel model);
    }
}