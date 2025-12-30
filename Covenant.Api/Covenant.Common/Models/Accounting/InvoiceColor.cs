namespace Covenant.Common.Models.Accounting
{
    public class InvoiceColor
    {
        public string HeaderTableColor { get; set; }
        public string ItemsTableColor { get; set; }
        public string FooterContentColor { get; set; }
        public static InvoiceColor Covenant => new InvoiceColor
        {
            HeaderTableColor = "#577533",
            ItemsTableColor = "#CFE2AA",
            FooterContentColor = "#A4CB7C"
        };
        public static InvoiceColor Sigook => new InvoiceColor
        {
            HeaderTableColor = "#2876BC",
            ItemsTableColor = "#E9ECF3",
            FooterContentColor = "#A9CEF0"
        };
    }
}
