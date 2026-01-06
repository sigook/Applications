using Covenant.Common.Models.Accounting;
using Covenant.HtmlTemplates.Views.Shared;

namespace Covenant.HtmlTemplates.Views.Reports.SummaryReport
{
    public class SummaryReportViewModel
    {
        public string ReportTo { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public int? FaxExt { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public long NumberId { get; set; }
        public IReadOnlyList<DescriptionTableViewModel> Items { get; set; } = new List<DescriptionTableViewModel>();
        public string AgencyFullName { get; set; }
        public string PhonePrincipal { get; set; }
        public int? PhonePrincipalExt { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string HtmlNotes { get; set; }
        public string AgencyLogoFileName { get; set; }
        public DateTime? WeekEnding { get; set; }
        public InvoiceColor InvoiceColor { get; set; }
    }
}