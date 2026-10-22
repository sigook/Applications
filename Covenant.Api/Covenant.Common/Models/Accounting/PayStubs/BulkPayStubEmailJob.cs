namespace Covenant.Common.Models.Accounting.PayStubs;

public record BulkPayStubEmailJob(Guid AgencyId, string Nickname, IReadOnlyList<Guid> PayStubIds);
