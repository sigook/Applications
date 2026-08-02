using IdentityModel.Client;
using Sigook.Functions.Configuration;
using Sigook.Functions.Models;

namespace Sigook.Functions.Utils;

public static class AccessToken
{
    private static TokenExpiryTime _authData;

    public static async Task<string> GetToken(this HttpClient client, ScheduleTasksOptions options)
    {
        if (_authData != null)
        {
            string token = _authData.Token;
            DateTime expiryTime = _authData.ExpiryTime;
            if (!string.IsNullOrEmpty(token) && expiryTime > DateTime.UtcNow) return token;
        }

        if (string.IsNullOrEmpty(options.AccountsUrl) || string.IsNullOrEmpty(options.ClientId) || string.IsNullOrEmpty(options.ClientSecret))
        {
            throw new InvalidOperationException($"{ScheduleTasksOptions.SectionName} credentials are not configured (AccountsUrl, ClientId, ClientSecret)");
        }

        TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = options.AccountsUrl,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Scope = "api1"
            });

        if (tokenResponse.IsError)
        {
            throw new InvalidOperationException($"Token request failed: {tokenResponse.Error} {tokenResponse.ErrorDescription}");
        }

        _authData = new TokenExpiryTime(tokenResponse.AccessToken, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));
        return _authData.Token;
    }
}
