namespace Covenant.Common.Models;

public record StartupFailure(Exception Exception, string Message, DateTime OccurredAt);