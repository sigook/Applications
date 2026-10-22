namespace Covenant.Common.Models.Accounting.Invoice
{
    public class InvoiceSummaryItemModel
    {
        public InvoiceSummaryItemModel() 
        { 
        }

        public InvoiceSummaryItemModel(string description, double quantity, decimal total)
        {
            Quantity = Math.Round(quantity, 2);
            Description = description;
            UnitPrice = Quantity <= 0 ? 0 : Math.Round(total / (decimal)Quantity, 2);
            Total = Math.Round((decimal)Quantity * UnitPrice, 2);
        }

        public InvoiceSummaryItemModel(string description, double quantity, decimal unitPrice, decimal total = 0)
        {
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Total = total == 0 ? (decimal)quantity * unitPrice : total;
        }

        public string Description { get; set; }
        public double Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal UnitPrice { get; set; }
    }
}