using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Subcontractor.Services;

namespace Covenant.Billing.Services
{
    public class AccountingCreateInvoiceAndReportsSubcontractor
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly ICreateInvoiceWithoutTimeSheet _createInvoiceWithoutTimeSheet;
        private readonly ICreateInvoiceUsingTimeSheet _createInvoiceUsingTimeSheet;
        private readonly CreateReportSubcontractorUsingTimeSheet _reportsSubcontractor;

        public AccountingCreateInvoiceAndReportsSubcontractor(
            ITimeSheetRepository timeSheetRepository,
            ICreateInvoiceWithoutTimeSheet createInvoiceWithoutTimeSheet,
            ICreateInvoiceUsingTimeSheet createInvoiceUsingTimeSheet,
            CreateReportSubcontractorUsingTimeSheet reportsSubcontractor)
        {
            _timeSheetRepository = timeSheetRepository;
            _createInvoiceWithoutTimeSheet = createInvoiceWithoutTimeSheet;
            _createInvoiceUsingTimeSheet = createInvoiceUsingTimeSheet;
            _reportsSubcontractor = reportsSubcontractor;
        }

        public async Task<Result<Invoice>> Create(CreateInvoiceModel model, Guid agencyId)
        {
            var agencyIds = new List<Guid> { agencyId };
            var timeSheet = await _timeSheetRepository.GetTimeSheetForCreatingInvoice(agencyIds, model);
            if (!timeSheet.Any()) return await _createInvoiceWithoutTimeSheet.Create(model.CompanyProfileId, model);

            var rInvoice = await _createInvoiceUsingTimeSheet.Create(agencyId, model.CompanyProfileId, timeSheet, model);
            if (!rInvoice) return rInvoice;

            var timeSheetPayroll = await _timeSheetRepository.GetTimeSheetForCreatingReportsSubcontractor(agencyIds, model.CompanyId);
            if (!timeSheet.Any()) return rInvoice;
            var rSubcontractor = await _reportsSubcontractor.Create(timeSheetPayroll);
            return rSubcontractor ? rInvoice : Result.Fail<Invoice>(rSubcontractor.Errors);
        }
    }
}