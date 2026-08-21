namespace Covenant.IdentityServer.Models;

public class PasswordResetResult
{
    public bool Succeeded { get; private init; }
    public string Error { get; private init; }
    public IReadOnlyCollection<string> Messages { get; private init; } = Array.Empty<string>();

    public static PasswordResetResult Success() => new() { Succeeded = true };

    public static PasswordResetResult Fail(string error, IReadOnlyCollection<string> messages = null) =>
        new() { Succeeded = false, Error = error, Messages = messages ?? Array.Empty<string>() };
}
