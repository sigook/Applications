using Covenant.Common.Models;

namespace Covenant.Deductions.Models
{
    public class DeductionPagination : Pagination
    {
        public int Year { get; set; }
        public string Province { get; set; }
    }
}