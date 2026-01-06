using System;

namespace Covenant.HtmlTemplates.Views.Billing.Payment
{
	public class PaymentSucceededViewModel
	{
		public string InvoiceNumber { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public DateTime TransactionDate { get; set; }
		public string TransactionId { get; set; }

		public PaymentSucceededViewModel()
		{
		}
		public PaymentSucceededViewModel(string invoiceNumber, decimal amount, string description)
		{
			InvoiceNumber = invoiceNumber;
			Amount = amount;
			Description = description;
		}
	}
}