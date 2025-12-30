using System;

namespace Covenant.HtmlTemplates.Views.Billing.Payroll
{
	public class PayrollEmailViewModel
	{
		public PayrollEmailViewModel(string workerFullName, decimal totalNet, DateTime endDate,
			DateTime paymentDate,string workerEmail,string payrollNumber)
		{
			WorkerFullName = workerFullName;
			TotalNet = totalNet;
			EndDate = endDate;
			PaymentDate = paymentDate;
			WorkerEmail = workerEmail;
			PayrollNumber = payrollNumber;
		}
		public string WorkerFullName { get; set; }
		public decimal TotalNet { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime PaymentDate { get; set; }
		public string WorkerEmail { get; set; }
		public string PayrollNumber { get; set; }

		public PayrollEmailViewModel()
		{
		}
	}
}