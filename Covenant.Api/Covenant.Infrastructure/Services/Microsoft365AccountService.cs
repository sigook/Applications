using Azure.Core;
using Azure.Identity;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Covenant.Infrastructure.Services;

public class Microsoft365AccountService(
    IHttpClientFactory httpClientFactory,
    IOptions<Microsoft365Configuration> microsoft365Options,
    IOptions<IdentityConfiguration> identityOptions,
    IMemoryCache cache,
    ILogger<Microsoft365AccountService> logger) : IMicrosoft365AccountService
{
    private const string GraphScope = "https://graph.microsoft.com/.default";
    private const string GraphUsersUrl = "https://graph.microsoft.com/v1.0/users/";
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(5);

    private readonly Lazy<TokenCredential> _credential = new(() => BuildCredential(microsoft365Options.Value.TenantId, identityOptions.Value));

    public async Task<bool> IsAccountEnabled(string objectId)
    {
        if (string.IsNullOrWhiteSpace(objectId)) return true;
        if (_credential.Value is null) return true;

        var cacheKey = $"m365-account-enabled:{objectId}";
        if (cache.TryGetValue(cacheKey, out bool enabled)) return enabled;

        enabled = await QueryAccountEnabled(objectId);
        cache.Set(cacheKey, enabled, CacheLifetime);
        return enabled;
    }

    private async Task<bool> QueryAccountEnabled(string objectId)
    {
        try
        {
            var token = await _credential.Value.GetTokenAsync(new TokenRequestContext([GraphScope]), CancellationToken.None);
            using var client = httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{GraphUsersUrl}{objectId}?$select=accountEnabled");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            using var response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning("Microsoft 365 account not found. ObjectId={ObjectId}", objectId);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Microsoft Graph returned {StatusCode} while checking account state. ObjectId={ObjectId}", response.StatusCode, objectId);
                return true;
            }

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (!document.RootElement.TryGetProperty("accountEnabled", out var accountEnabled)) return true;

            var enabled = accountEnabled.ValueKind != JsonValueKind.False;
            if (!enabled) logger.LogWarning("Microsoft 365 account is disabled. ObjectId={ObjectId}", objectId);
            return enabled;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking Microsoft 365 account state. ObjectId={ObjectId}", objectId);
            return true;
        }
    }

    private static TokenCredential BuildCredential(string tenantId, IdentityConfiguration identity)
    {
        if (string.IsNullOrWhiteSpace(tenantId)
            || string.IsNullOrWhiteSpace(identity.Microsoft365ClientId)
            || string.IsNullOrWhiteSpace(identity.Microsoft365ClientSecret))
        {
            return null;
        }
        return new ClientSecretCredential(tenantId, identity.Microsoft365ClientId, identity.Microsoft365ClientSecret);
    }
}
