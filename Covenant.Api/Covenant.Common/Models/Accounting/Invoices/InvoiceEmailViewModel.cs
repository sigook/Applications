namespace Covenant.Common.Models.Accounting.Invoices;

public class InvoiceEmailViewModel
{
    public string AgencyAddress { get; set; }
    public string AgencyPhone { get; set; }
    public string AgencyFax { get; set; }
    public string AgencyWebSite { get; set; }
    public string AgencyLogoFileName { get; set; }
    public InvoicePayroll InvoicePayroll { get; set; }
    public string Message { get; set; }
}
