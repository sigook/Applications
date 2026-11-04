using ModelContextProtocol;
namespace Sigook.Mcp.Configuration;

public sealed record SigookEnvironment(string Name, Uri ApiUrl, Uri Authority, bool AllowInvalidCertificates)
{
    public const string ClientId = "sigook.mcp";
    public const string Scope = "openid profile email roles api1 offline_access";
    public const int LoopbackPort = 53421;
    public static readonly Uri RedirectUri = new($"http://localhost:{LoopbackPort}/callback");

    private static readonly Dictionary<string, (string Api, string Authority)> Known = new(StringComparer.OrdinalIgnoreCase)
    {
        ["local"] = ("https://localhost:44307", "https://localhost:44307"),
        ["staging"] = ("https://staging.api.sigook.ca", "https://staging.accounts.sigook.ca"),
        ["production"] = ("https://api.sigook.com", "https://accounts.sigook.com")
    };

    public static SigookEnvironment FromProcessEnvironment()
    {
        var name = Environment.GetEnvironmentVariable("SIGOOK_ENV") ?? "staging";
        if (!Known.TryGetValue(name, out var urls))
            throw new McpException($"Unknown SIGOOK_ENV '{name}'. Use one of: {string.Join(", ", Known.Keys)}.");
        var api = Environment.GetEnvironmentVariable("SIGOOK_API_URL") ?? urls.Api;
        var authority = Environment.GetEnvironmentVariable("SIGOOK_AUTHORITY") ?? urls.Authority;
        var insecure = Environment.GetEnvironmentVariable("SIGOOK_INSECURE") == "1" || name.Equals("local", StringComparison.OrdinalIgnoreCase);
        return new SigookEnvironment(name.ToLowerInvariant(), new Uri(api.TrimEnd('/') + "/"), new Uri(authority.TrimEnd('/') + "/"), insecure);
    }

    public HttpMessageHandler CreateHandler()
    {
        var handler = new HttpClientHandler();
        if (AllowInvalidCertificates)
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        return handler;
    }

    public static string DataDirectory
    {
        get
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var directory = Path.Combine(root, "Sigook.Mcp");
            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}
