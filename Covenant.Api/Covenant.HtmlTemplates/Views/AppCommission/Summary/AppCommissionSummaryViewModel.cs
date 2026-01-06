using System;
using System.Collections.Generic;
using Covenant.HtmlTemplates.Views.Shared;

namespace Covenant.HtmlTemplates.Views.AppCommission.Summary
{
    public class AppCommissionSummaryViewModel
    {
        public long NumberId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AgencyFullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public int? FaxExt { get; set; }
        public string PhonePrincipal { get; set; }
        public int? PhonePrincipalExt { get; set; }
        public IEnumerable<DescriptionTableViewModel> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}