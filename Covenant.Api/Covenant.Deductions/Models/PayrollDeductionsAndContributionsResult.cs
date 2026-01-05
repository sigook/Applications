namespace Covenant.Deductions.Models
{
	public readonly struct PayrollDeductionsAndContributionsResult
	{
		public PayrollDeductionsAndContributionsResult(decimal cpp, decimal ei, decimal federalTax, decimal provincialTax)
		{
			Cpp = cpp;
			Ei = ei;
			FederalTax = federalTax;
			ProvincialTax = provincialTax;
			Total = cpp + ei + federalTax + provincialTax;
		}

		public decimal Cpp { get; }
		public decimal Ei { get; }
		public decimal FederalTax { get; }
		public decimal ProvincialTax { get; }
		public decimal Total { get; }
	}
}