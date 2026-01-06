using System;

namespace Covenant.Deductions.Entities
{
	public class CppMonthly : ICpp
	{
		private CppMonthly(){}
		public CppMonthly(decimal @from, decimal to, decimal cpp, int year, Guid? id = null)
		{
			Id = id ?? Guid.NewGuid();
			From = @from;
			To = to;
			Cpp = cpp;
			Year = year;
		}

		public CppMonthly(ICpp iCpp, int year)
		{
			if (iCpp == null) throw new ArgumentNullException(nameof(iCpp));
			Id= iCpp.Id == Guid.Empty ? Guid.NewGuid() : iCpp.Id;
			From = iCpp.From;
			To = iCpp.To;
			Cpp = iCpp.Cpp;
			Year = year;			
		}

		public Guid Id { get; private set; }
		public decimal From { get; private set; }
		public decimal To { get; private set; }
		public decimal Cpp { get; private set; }
		public int Year { get; private set; }
	}
}