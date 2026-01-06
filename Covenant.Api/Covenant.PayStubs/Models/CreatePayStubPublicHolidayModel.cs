using System;

namespace Covenant.PayStubs.Models
{
	public class CreatePayStubPublicHolidayModel
	{
		public DateTime Holiday { get; set; }
		public decimal Amount { get; set; }
	}
}