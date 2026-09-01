using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Core.BL.Adapters;

public class PayrollDocumentAdapter : IPayrollDocumentAdapter
{
    public PayrollViewModel MapToPayrollViewModel(PayStubDetailModel model)
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

    public PayrollEmailViewModel MapToPayrollEmailViewModel(PayStubDetailModel model)
    {
        return new PayrollEmailViewModel
        {
            WorkerFullName = model.WorkerFullName,
            TotalNet = model.TotalNet,
            EndDate = model.EndDate,
            PaymentDate = model.PaymentDate,
            WorkerEmail = model.WorkerEmail,
            PayrollNumber = model.PayrollNumber
        };
    }

    private static IEnumerable<PayrollTable2Item> GetItemsTable2(PayStubDetailModel model)
    {
        return GetPlusValues().Concat(
        [
            PayrollTable2Item.EmptyRow,
            new PayrollTable2Item("CPP (-)", model.DeductionCpp.ToCaMoney()),
            new PayrollTable2Item("EI (-)", model.DeductionEi.ToCaMoney()),
            new PayrollTable2Item($"Federal TAX {model.FederalCategory.ToString().ToUpper()} (-)", model.DeductionTax.ToCaMoney()),
            new PayrollTable2Item($"Provincial TAX {model.ProvincialCategory.ToString().ToUpper()} (-)", model.DeductionProvincialTax.ToCaMoney())
        ]).Concat(GetOtherDeductions()).Concat(
            [
                new PayrollTable2Item("Current Deductions (-)", model.DeductionTotal.ToCaMoney()),
                PayrollTable2Item.EmptyRow,
                new PayrollTable2Item("Total Net Paid", model.TotalNet.ToCaMoney())
            ]);

        IEnumerable<PayrollTable2Item> GetPlusValues()
        {
            var result = new List<PayrollTable2Item>(4)
            {
                new PayrollTable2Item("Gross Payment (+)", model.Gross.ToCaMoney()),
                new PayrollTable2Item("Vacations (+)", model.Vacations.ToCaMoney())
            };
            result.Add(new PayrollTable2Item("Total Earnings (+)", model.Earnings.ToCaMoney()));
            return result;
        }

        IEnumerable<PayrollTable2Item> GetOtherDeductions()
        {
            if (model.OtherDeductions.Count != 0)
            {
                return model.OtherDeductions.Select(s =>
                     new PayrollTable2Item(string.IsNullOrEmpty(s.Description) ? "Others Deductions (-)" : s.Description, s.Total.ToCaMoney()));
            }
            return [];
        }
    }

    private static IEnumerable<PayrollTable2Item> GetYtdItems(PayStubYtdModel ytd)
    {
        return
        [
            new PayrollTable2Item("Gross Payment", ytd.Gross.ToCaMoney()),
            new PayrollTable2Item("Vacations", ytd.Vacations.ToCaMoney()),
            new PayrollTable2Item("Total Earnings", ytd.Earnings.ToCaMoney()),
            PayrollTable2Item.EmptyRow,
            new PayrollTable2Item("CPP", ytd.Cpp.ToCaMoney()),
            new PayrollTable2Item("EI", ytd.Ei.ToCaMoney()),
            new PayrollTable2Item("Federal Tax", ytd.FederalTax.ToCaMoney()),
            new PayrollTable2Item("Provincial Tax", ytd.ProvincialTax.ToCaMoney()),
            new PayrollTable2Item("Total Deductions", ytd.TotalDeductions.ToCaMoney()),
            PayrollTable2Item.EmptyRow,
            new PayrollTable2Item("Total Net Paid", ytd.TotalPaid.ToCaMoney())
        ];
    }
}
