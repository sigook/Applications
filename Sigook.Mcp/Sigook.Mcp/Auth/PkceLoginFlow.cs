using ModelContextProtocol;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sigook.Mcp.Configuration;

namespace Sigook.Mcp.Auth;

public sealed class TokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
    [JsonPropertyName("error")] public string? Error { get; set; }
    [JsonPropertyName("error_description")] public string? ErrorDescription { get; set; }
}

public sealed class PkceLoginFlow(HttpClient http, SigookEnvironment environment, ILogger<PkceLoginFlow> logger)
{
    private static readonly TimeSpan LoginTimeout = TimeSpan.FromMinutes(3);

    public async Task<TokenResponse> LoginAsync(CancellationToken cancellationToken)
    {
        var verifier = Base64Url(RandomNumberGenerator.GetBytes(32));
        var challenge = Base64Url(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));
        var state = Base64Url(RandomNumberGenerator.GetBytes(16));

        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{SigookEnvironment.LoopbackPort}/callback/");
        try
        {
            listener.Start();
        }
        catch (HttpListenerException ex)
        {
            throw new McpException($"Port {SigookEnvironment.LoopbackPort} is busy; close the process using it and retry. ({ex.Message})");
        }

        var authorizeUrl = BuildAuthorizeUrl(challenge, state);
        logger.LogInformation("Opening browser for Sigook sign-in ({Environment})", environment.Name);
        OpenBrowser(authorizeUrl);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(LoginTimeout);
        var context = await listener.GetContextAsync().WaitAsync(timeout.Token);
        var query = context.Request.QueryString;
        var code = query["code"];
        var returnedState = query["state"];
        var error = query["error"];

        await RespondAsync(context.Response, string.IsNullOrEmpty(error) ? "Signed in to Sigook. You can close this window." : $"Sign-in failed: {error}");
        listener.Stop();

        if (!string.IsNullOrEmpty(error)) throw new McpException($"Authorization failed: {error} {query["error_description"]}");
        if (string.IsNullOrEmpty(code) || returnedState != state) throw new McpException("Authorization response is invalid.");

        return await ExchangeAsync(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = SigookEnvironment.RedirectUri.ToString(),
            ["code_verifier"] = verifier
        }, cancellationToken);
    }

    public Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken) =>
        ExchangeAsync(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        }, cancellationToken);

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = SigookEnvironment.ClientId,
                ["token"] = refreshToken,
                ["token_type_hint"] = "refresh_token"
            });
            await http.PostAsync(new Uri(environment.Authority, "connect/revocation"), content, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Refresh token revocation failed");
        }
    }

    private async Task<TokenResponse> ExchangeAsync(Dictionary<string, string> form, CancellationToken cancellationToken)
    {
        form["client_id"] = SigookEnvironment.ClientId;
        using var content = new FormUrlEncodedContent(form);
        using var response = await http.PostAsync(new Uri(environment.Authority, "connect/token"), content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokens = JsonSerializer.Deserialize<TokenResponse>(body) ?? new TokenResponse { Error = "invalid_response" };
        if (!response.IsSuccessStatusCode || !string.IsNullOrEmpty(tokens.Error))
            throw new McpException($"Token request failed ({(int)response.StatusCode}): {tokens.Error} {tokens.ErrorDescription}");
        return tokens;
    }

    private string BuildAuthorizeUrl(string challenge, string state)
    {
        var query = new Dictionary<string, string>
        {
            ["client_id"] = SigookEnvironment.ClientId,
            ["response_type"] = "code",
            ["redirect_uri"] = SigookEnvironment.RedirectUri.ToString(),
            ["scope"] = SigookEnvironment.Scope,
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["state"] = state
        };
        var queryString = string.Join("&", query.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        return new Uri(environment.Authority, $"connect/authorize?{queryString}").ToString();
    }

    private static void OpenBrowser(string url)
    {
        if (OperatingSystem.IsWindows())
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        else if (OperatingSystem.IsMacOS())
            Process.Start("open", url);
        else
            Process.Start("xdg-open", url);
    }

    private static async Task RespondAsync(HttpListenerResponse response, string message)
    {
        var html = $"<!doctype html><html><body style=\"font-family:sans-serif;padding:2rem\"><h2>{WebUtility.HtmlEncode(message)}</h2></body></html>";
        var bytes = Encoding.UTF8.GetBytes(html);
        response.ContentType = "text/html; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes);
        response.Close();
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
