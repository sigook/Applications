namespace Covenant.Common.Models.Accounting.Invoice;

public record InvoiceDocument(string PdfPath, string FileName, InvoiceSummaryModel Model);
