using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;

namespace Covenant.Api.AccountingModule.InvoiceDocument.Models
{
    internal static class InvoiceMappers
    {
        internal static InvoiceViewModel ToInvoiceViewModel(this InvoiceSummaryModel model)
        {
            List<DescriptionTableViewModel> items = model.Items.Select(i => new DescriptionTableViewModel
            {
                Description = i.Description,
                Total = i.Total,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();
            List<DescriptionTableViewModel> discounts = model.Discounts.Select(d => new DescriptionTableViewModel
            {
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Total = d.Amount * -1,
                Description = d.Description
            }).ToList();

            List<DescriptionTableViewModel> holidays = model.Holidays.GroupBy(h => h.UnitPrice).Select(m => new DescriptionTableViewModel
            {
                UnitPrice = m.Key,
                Total = m.Sum(t => t.Amount),
                Quantity = m.Sum(q => q.Hours),
                Description = string.Join(", ", m.GroupBy(d => d.Description).Select(h => h.Key))
            }).ToList();

            List<DescriptionTableViewModel> additionalItems = model.AdditionalItems.Select(h => new DescriptionTableViewModel
            {
                Total = h.Total,
                Quantity = h.Quantity,
                Description = h.Description,
                UnitPrice = h.UnitPrice
            }).ToList();

            return new InvoiceViewModel
            {
                Id = model.Id,
                CompanyProfileId = model.CompanyProfileId,
                NumberId = model.NumberId,
                InvoiceNumber = model.InvoiceNumber,
                Fax = model.Fax,
                FaxExt = model.FaxExt,
                Email = model.Email,
                PhonePrincipal = model.PhonePrincipal,
                PhonePrincipalExt = model.PhonePrincipalExt,
                Total = model.Total,
                Address = model.Address,
                HtmlNotes = model.HtmlNotes,
                SubTotal = model.SubTotal,
                CreatedAt = model.CreatedAt,
                AgencyFullName = model.AgencyFullName,
                AgencyLogoFileName = model.AgencyLogoFileName,
                AgencyAddress = model.AgencyAddress,
                AgencyPhone = model.AgencyPhone,
                AgencyPhoneExt = model.AgencyPhoneExt,
                AgencyFax = model.AgencyFax,
                AgencyFaxExt = model.AgencyFaxExt,
                BillTo = model.CompanyFullName,
                HstNumber = model.HstNumber,
                Hst = model.Hst,
                TaxName = model.TaxName,
                WeekEnding = model.WeedEnding,
                InvoiceColor = model.InvoiceColor,
                Items = items.Concat(discounts).Concat(holidays).Concat(additionalItems).ToList(),
            };
        }
    }
}