namespace Covenant.Common.Models.Accounting.PayStub;

public record BulkPayStubEmailJob(Guid AgencyId, string Nickname, IReadOnlyList<Guid> PayStubIds);
