using Covenant.Common.Models;

namespace Covenant.Common.Models.Accounting.Deductions
{
    public class DeductionPagination : Pagination
    {
        public int Year { get; set; }
        public string Province { get; set; }
    }
}