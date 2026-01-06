using System.ComponentModel.DataAnnotations;

namespace Covenant.Api.AccountingModule.Deduction
{
	public class CreateDeductionModel
	{
		public IFormFile File { get; set; }
		[Range(2000, 2100)]
		public int Year { get; set; }
	}
}