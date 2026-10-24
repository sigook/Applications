namespace Covenant.Common.Models.Accounting.PayStubs;

public record PayStubDocument(byte[] Content, string FileName, PayrollEmailViewModel Model);
