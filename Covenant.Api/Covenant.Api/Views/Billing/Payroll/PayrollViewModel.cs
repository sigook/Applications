using System;
using System.Collections.Generic;
using System.Linq;

namespace Covenant.HtmlTemplates.Views.Billing.Payroll
{
    public class PayrollViewModel
    {
        public string DocumentName { get; set; } = "Pay Stub";
        public long NumberId { get; set; }
        public string PayrollNumber { get; set; }
        public string AgencyLocation { get; set; }
        public string AgencyPhone { get; set; }
        public string WorkerFullName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AgencyFullName { get; set; }
        public int? AgencyPhoneExt { get; set; }
        public string AgencyLogoFileName { get; set; }
        public string WorkerEmail { get; set; }
        public string TypeOfJob { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PaymentDate { get; set; }
        public IEnumerable<PayrollTable1Item> Table1Items { get; set; } = new List<PayrollTable1Item>();
        public IEnumerable<PayrollTable2Item> Table2Items { get; set; } = new List<PayrollTable2Item>();

        public (string left,string right) GetBorderRadiosTable2(int i)
        {
            if (i == 0) return ("border-radius: 14px 0 0 0;","border-radius: 0 14px 0 0;");
            return i + 1 == Table2Items.Count() ? ("border-radius: 0 0 0 14px;","border-radius: 0 0 14px 0;") : ("","");
        }
    }
    
    public class PayrollTable1Item
    {
        public PayrollTable1Item(double quantity, decimal total,string description)
        {
            if(quantity <= 0) return;
            Description = description;
            Quantity = quantity;
            Total = total;
            UnitPrice = total / (decimal) quantity;
        }
        
        public double Quantity { get;  set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get;  set; }
        public string Description { get; set; }
        public PayrollTable1Item()
        {
        }
    }

    public class PayrollTable2Item
    {
        public PayrollTable2Item()
        {
        }
        public PayrollTable2Item(string label, string value)
        {
            Label = label;
            Value = value;
        }
        public string Label { get; set; }
        public string Value { get; set; }

        public static readonly PayrollTable2Item EmptyRow = new PayrollTable2Item(string.Empty, string.Empty);
    }
}