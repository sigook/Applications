namespace Covenant.Common.Models.Accounting.Invoice;

public record InvoiceDocument(byte[] Content, string FileName, InvoiceSummaryModel Model);
