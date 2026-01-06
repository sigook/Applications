using System;

namespace Covenant.Deductions.Entities
{
	public class ProvincialTaxBiWeekly : IProvincialTax
	{
		private ProvincialTaxBiWeekly(){}
		public ProvincialTaxBiWeekly(decimal @from, 
			decimal to,
			decimal? cc0, 
			decimal? cc1,
			decimal? cc2,
			decimal? cc3, 
			decimal? cc4, 
			decimal? cc5, 
			decimal? cc6, 
			decimal? cc7, 
			decimal? cc8, 
			decimal? cc9, 
			decimal? cc10, 
			int year,
			Guid? id = null)
		{
			From = @from;
			To = to;
			Cc0 = cc0;
			Cc1 = cc1;
			Cc2 = cc2;
			Cc3 = cc3;
			Cc4 = cc4;
			Cc5 = cc5;
			Cc6 = cc6;
			Cc7 = cc7;
			Cc8 = cc8;
			Cc9 = cc9;
			Cc10 = cc10;
			Year = year;
			Id = id ?? Guid.NewGuid();
		}

		public ProvincialTaxBiWeekly(ITax tax)
		{
			if (tax is null) throw new ArgumentNullException(nameof(tax));
			From = tax.From;
			To = tax.To;
			Cc0 = tax.Cc0;
			Cc1 = tax.Cc1;
			Cc2 = tax.Cc2;
			Cc3 = tax.Cc3;
			Cc4 = tax.Cc4;
			Cc5 = tax.Cc5;
			Cc6 = tax.Cc6;
			Cc7 = tax.Cc7;
			Cc8 = tax.Cc8;
			Cc9 = tax.Cc9;
			Cc10 = tax.Cc10;
			Year = tax.Year;
			Id = tax.Id == Guid.Empty ? Guid.NewGuid() : tax.Id;
		}

		public Guid Id { get; private set; }
		public decimal From { get; private set; }
		public decimal To { get; private set; }
		public decimal? Cc0 { get; private set; }
		public decimal? Cc1 { get; private set; }
		public decimal? Cc2 { get; private set; }
		public decimal? Cc3 { get;private set; }
		public decimal? Cc4 { get;private set; }
		public decimal? Cc5 { get;private set; }
		public decimal? Cc6 { get;private set; }
		public decimal? Cc7 { get;private set; }
		public decimal? Cc8 { get;private set; }
		public decimal? Cc9 { get;private set; }
		public decimal? Cc10 { get;private set; }
		public int Year { get; private set; }
	}
}