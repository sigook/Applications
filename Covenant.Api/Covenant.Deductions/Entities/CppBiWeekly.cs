using System;

namespace Covenant.Deductions.Entities
{
	public class CppBiWeekly : ICpp
	{
		private CppBiWeekly(){}
		public CppBiWeekly(decimal @from, decimal to, decimal cpp, int year, Guid? id = null)
		{
			Id = id ?? Guid.NewGuid();
			From = @from;
			To = to;
			Cpp = cpp;
			Year = year;
		}

        public CppBiWeekly(ICpp icpp, int year)
        {
			if (icpp == null) throw new ArgumentNullException(nameof(icpp));
			Id = icpp.Id == Guid.Empty ? Guid.NewGuid() : icpp.Id;
			From = icpp.From;
			To = icpp.To;
			Cpp = icpp.Cpp;
			Year = year;
        }

        public Guid Id { get; private set; }
		public decimal From { get; private set; }
		public decimal To { get; private set; }
		public decimal Cpp { get; private set; }
		public int Year { get; private set; }
	}
}