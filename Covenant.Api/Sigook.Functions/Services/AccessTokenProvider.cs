using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Sigook.Functions.Configuration;

namespace Sigook.Functions.Services;

public class AccessTokenProvider(IHttpClientFactory httpClientFactory, IOptions<ScheduleTasksOptions> options) : IAccessTokenProvider
{
    private const string Scope = "api1";
    private static readonly TimeSpan ExpirationMargin = TimeSpan.FromSeconds(30);

    private readonly ScheduleTasksOptions _options = options.Value;
    private readonly SemaphoreSlim _refresh = new(1, 1);
    private string _token;
    private DateTime _expiresAt;

    public async Task<string> GetToken()
    {
        if (HasValidToken) return _token;

        await _refresh.WaitAsync();
        try
        {
            if (HasValidToken) return _token;

            if (string.IsNullOrEmpty(_options.AccountsUrl) || string.IsNullOrEmpty(_options.ClientId) || string.IsNullOrEmpty(_options.ClientSecret))
            {
                throw new InvalidOperationException($"{ScheduleTasksOptions.SectionName} credentials are not configured (AccountsUrl, ClientId, ClientSecret)");
            }

            var client = httpClientFactory.CreateClient(HttpClients.Api);
            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _options.AccountsUrl,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Scope = Scope
            });

            if (response.IsError)
            {
                throw new InvalidOperationException($"Token request failed: {response.Error} {response.ErrorDescription}");
            }

            _token = response.AccessToken;
            _expiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn) - ExpirationMargin;
            return _token;
        }
        finally
        {
            _refresh.Release();
        }
    }

    private bool HasValidToken => !string.IsNullOrEmpty(_token) && _expiresAt > DateTime.UtcNow;
}
