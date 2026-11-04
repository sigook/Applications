using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Sigook.Mcp.Configuration;

namespace Sigook.Mcp.Auth;

public sealed record StoredTokens(string AccessToken, string? RefreshToken, DateTimeOffset ExpiresAt, string Environment);

public sealed class TokenStore(SigookEnvironment environment)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string path = Path.Combine(SigookEnvironment.DataDirectory, "tokens", $"{environment.Name}.bin");

    public StoredTokens? Load()
    {
        if (!File.Exists(path)) return null;
        try
        {
            var bytes = File.ReadAllBytes(path);
            var json = Encoding.UTF8.GetString(Unprotect(bytes));
            return JsonSerializer.Deserialize<StoredTokens>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public void Save(StoredTokens tokens)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var json = JsonSerializer.Serialize(tokens, JsonOptions);
        File.WriteAllBytes(path, Protect(Encoding.UTF8.GetBytes(json)));
        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);
    }

    public void Clear()
    {
        if (File.Exists(path)) File.Delete(path);
    }

    private static byte[] Protect(byte[] data) =>
        OperatingSystem.IsWindows() ? ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser) : data;

    private static byte[] Unprotect(byte[] data) =>
        OperatingSystem.IsWindows() ? ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser) : data;
}
