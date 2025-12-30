using Covenant.Common.Models.Accounting.PayStub;
using Covenant.HtmlTemplates.Views.Billing.Payroll;

namespace Covenant.Api.Shared.PayrollDocument.Models
{
    internal static class PayrollMappers
    {
        internal static PayrollViewModel ToPayrollViewModel(this PayStubDetailModel model)
        {
            return new PayrollViewModel
            {
                NumberId = model.NumberId,
                PayrollNumber = model.PayrollNumber,
                AgencyPhone = model.AgencyPhone,
                AgencyPhoneExt = model.AgencyPhoneExt,
                AgencyLocation = model.AgencyLocation,
                AgencyFullName = model.AgencyFullName,
                AgencyLogoFileName = model.AgencyLogoFileName,
                WorkerFullName = model.WorkerFullName,
                WorkerEmail = model.WorkerEmail,
                TypeOfJob = model.TypeOfJob,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreatedAt = model.CreatedAt,
                PaymentDate = model.PaymentDate,
                Table1Items = model.Items.Select((c, i) => new PayrollTable1Item
                {
                    Description = c.Description,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                    Total = c.Total,
                }).ToList(),
                Table2Items = GetItemsTable2(model)
            };
        }

        private static IEnumerable<PayrollTable2Item> GetItemsTable2(PayStubDetailModel model)
        {
            return GetPlusValues().Concat(new[]
            {
                PayrollTable2Item.EmptyRow,
                new PayrollTable2Item("CPP (-)", model.DeductionCpp.ToString("C")),
                new PayrollTable2Item("EI (-)", model.DeductionEi.ToString("C")),
                new PayrollTable2Item($"Federal TAX {model.FederalCategory.ToString().ToUpper()} (-)", model.DeductionTax.ToString("C")),
                new PayrollTable2Item($"Provincial TAX {model.ProvincialCategory.ToString().ToUpper()} (-)", model.DeductionProvincialTax.ToString("C"))
            }).Concat(GetOtherDeductions()).Concat(new[]
                {
                    new PayrollTable2Item("Total Deductions (-)",model.DeductionTotal.ToString("C")),
                    PayrollTable2Item.EmptyRow,
                    new PayrollTable2Item("Total Paid",model.TotalNet.ToString("C"))
                });

            IEnumerable<PayrollTable2Item> GetPlusValues()
            {
                var result = new List<PayrollTable2Item>(4)
                {
                    new PayrollTable2Item("Gross Payment (+)", model.Gross.ToString("C")),
                    new PayrollTable2Item("Vacations (+)", model.Vacations.ToString("C"))
                };
                if (model.Holiday > 0) result.Add(new PayrollTable2Item("Holidays (+)", model.Holiday.ToString("C")));
                result.Add(new PayrollTable2Item("Total Earnings (+)", model.Earnings.ToString("C")));
                return result;
            }

            IEnumerable<PayrollTable2Item> GetOtherDeductions()
            {
                if (model.OtherDeductionsDetail.Any())
                {
                    return model.OtherDeductionsDetail.Select(s =>
                         new PayrollTable2Item(string.IsNullOrEmpty(s.Description) ? "Others Deductions (-)" : s.Description, s.Total.ToString("C")));
                }
                return model.DeductionOthers > 0 ? new[] { new PayrollTable2Item("Others Deductions (-)", model.DeductionOthers.ToString("C")) } : Array.Empty<PayrollTable2Item>();
            }
        }

        internal static PayrollViewModel ToPayrollReportViewModel(this PayStubDetailModel model)
        {
            var result = model.ToPayrollViewModel();
            result.DocumentName = "Report";
            result.PayrollNumber = model.NumberId.ToString("0000");
            return result;
        }
    }
}