using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace Covenant.IdentityServer.Services.Impl;

public class Microsoft365AccountService : IMicrosoft365AccountService
{
    private const string GraphScope = "https://graph.microsoft.com/.default";
    private const string GraphUsersUrl = "https://graph.microsoft.com/v1.0/users/";
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(5);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<Microsoft365AccountService> _logger;
    private readonly Lazy<TokenCredential> _credential;

    public Microsoft365AccountService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<Microsoft365AccountService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
        _credential = new Lazy<TokenCredential>(() => BuildCredential(configuration));
    }

    public async Task<bool> IsAccountEnabledAsync(string objectId)
    {
        if (string.IsNullOrWhiteSpace(objectId)) return true;
        if (_credential.Value is null) return true;

        string cacheKey = $"m365-account-enabled:{objectId}";
        if (_cache.TryGetValue(cacheKey, out bool enabled)) return enabled;

        enabled = await QueryAccountEnabledAsync(objectId);
        _cache.Set(cacheKey, enabled, CacheLifetime);
        return enabled;
    }

    private async Task<bool> QueryAccountEnabledAsync(string objectId)
    {
        try
        {
            AccessToken token = await _credential.Value.GetTokenAsync(new TokenRequestContext(new[] { GraphScope }), CancellationToken.None);
            using HttpClient client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{GraphUsersUrl}{objectId}?$select=accountEnabled");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            using HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Microsoft 365 account not found. ObjectId={ObjectId}", objectId);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Microsoft Graph returned {StatusCode} while checking account state. ObjectId={ObjectId}", response.StatusCode, objectId);
                return true;
            }

            using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (!document.RootElement.TryGetProperty("accountEnabled", out JsonElement accountEnabled)) return true;

            bool enabled = accountEnabled.ValueKind != JsonValueKind.False;
            if (!enabled) _logger.LogWarning("Microsoft 365 account is disabled. ObjectId={ObjectId}", objectId);
            return enabled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Microsoft 365 account state. ObjectId={ObjectId}", objectId);
            return true;
        }
    }

    private static TokenCredential BuildCredential(IConfiguration configuration)
    {
        string authority = configuration.GetValue<string>("Microsoft365Authority");
        string clientId = configuration.GetValue<string>("Microsoft365ClientId");
        string clientSecret = configuration.GetValue<string>("Microsoft365ClientSecret");
        if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret)) return null;

        string tenantId = new Uri(authority).Segments.Skip(1).FirstOrDefault()?.Trim('/');
        if (string.IsNullOrWhiteSpace(tenantId)) return null;

        return new ClientSecretCredential(tenantId, clientId, clientSecret);
    }
}
