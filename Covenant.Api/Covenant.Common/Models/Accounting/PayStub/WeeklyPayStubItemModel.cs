namespace Covenant.Common.Models.Accounting.PayStub
{
	public class WeeklyPayStubItemModel
	{
		public WeeklyPayStubItemModel()
		{
		}

		public WeeklyPayStubItemModel(string description, double quantity, decimal unitPrice, decimal total)
		{
			Description = description;
			Quantity = quantity;
			UnitPrice = unitPrice;
			Total = total;
		}
		public WeeklyPayStubItemModel(string description, decimal quantity, decimal unitPrice, decimal total)
		{
			Description = description;
			Quantity = (double)quantity;
			UnitPrice = unitPrice;
			Total = total;
		}
		public string Description { get; set; }
		public double Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Total { get; set; }
	}
}