namespace Covenant.Common.Models.Accounting.PayStub;

public record PayStubDocument(byte[] Content, string FileName, PayrollEmailViewModel Model);
