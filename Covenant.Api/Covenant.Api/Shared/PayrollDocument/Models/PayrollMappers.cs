using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Utils.Extensions;
using Covenant.HtmlTemplates.Views.Billing.Payroll;

namespace Covenant.Api.Shared.PayrollDocument.Models;

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
            MaskedSin = model.SinNumber.MaskSIN(),
            EmployeeId = model.EmployeeId,
            WorkerEmail = model.WorkerEmail,
            Position = model.Position,
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
            Table2Items = GetItemsTable2(model),
            YtdItems = model.Ytd != null ? GetYtdItems(model.Ytd) : []
        };
    }

    private static IEnumerable<PayrollTable2Item> GetItemsTable2(PayStubDetailModel model)
    {
        return GetPlusValues().Concat(
        [
            PayrollTable2Item.EmptyRow,
            new PayrollTable2Item("CPP (-)", model.DeductionCpp.ToString("C")),
            new PayrollTable2Item("EI (-)", model.DeductionEi.ToString("C")),
            new PayrollTable2Item($"Federal TAX {model.FederalCategory.ToString().ToUpper()} (-)", model.DeductionTax.ToString("C")),
            new PayrollTable2Item($"Provincial TAX {model.ProvincialCategory.ToString().ToUpper()} (-)", model.DeductionProvincialTax.ToString("C"))
        ]).Concat(GetOtherDeductions()).Concat(
            [
                new PayrollTable2Item("Current Deductions (-)", model.DeductionTotal.ToString("C")),
                PayrollTable2Item.EmptyRow,
                new PayrollTable2Item("Total Net Paid",model.TotalNet.ToString("C"))
            ]);

        IEnumerable<PayrollTable2Item> GetPlusValues()
        {
            var result = new List<PayrollTable2Item>(4)
            {
                new PayrollTable2Item("Gross Payment (+)", model.Gross.ToString("C")),
                new PayrollTable2Item("Vacations (+)", model.Vacations.ToString("C"))
            };
            result.Add(new PayrollTable2Item("Total Earnings (+)", model.Earnings.ToString("C")));
            return result;
        }

        IEnumerable<PayrollTable2Item> GetOtherDeductions()
        {
            if (model.OtherDeductions.Count != 0)
            {
                return model.OtherDeductions.Select(s =>
                     new PayrollTable2Item(string.IsNullOrEmpty(s.Description) ? "Others Deductions (-)" : s.Description, s.Total.ToString("C")));
            }
            return [];
        }
    }

    private static IEnumerable<PayrollTable2Item> GetYtdItems(PayStubYtdModel ytd)
    {
        return
        [
            new PayrollTable2Item("Gross Payment", ytd.Gross.ToString("C")),
            new PayrollTable2Item("Vacations", ytd.Vacations.ToString("C")),
            new PayrollTable2Item("Total Earnings", ytd.Earnings.ToString("C")),
            PayrollTable2Item.EmptyRow,
            new PayrollTable2Item("CPP", ytd.Cpp.ToString("C")),
            new PayrollTable2Item("EI", ytd.Ei.ToString("C")),
            new PayrollTable2Item("Federal Tax", ytd.FederalTax.ToString("C")),
            new PayrollTable2Item("Provincial Tax", ytd.ProvincialTax.ToString("C")),
            new PayrollTable2Item("Total Deductions", ytd.TotalDeductions.ToString("C")),
            PayrollTable2Item.EmptyRow,
            new PayrollTable2Item("Total Net Paid", ytd.TotalPaid.ToString("C"))
        ];
    }
}