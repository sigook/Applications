using IdentityModel.Client;
using Microsoft.Extensions.Options;
using SigookFunctions.Models;
using SigookFunctions.Options;

namespace SigookFunctions.Services
{
    public class TokenService : ITokenService
    {
        private readonly SigookFunctionsOptions _options;
        private readonly HttpClient _client;
        private TokenExpiryTime? _authData;

        public TokenService(IOptions<SigookFunctionsOptions> options)
        {
            _options = options.Value;
            _client = new HttpClient();
        }

        public async Task<string> GetTokenAsync()
        {
            if (_authData != null)
            {
                string token = _authData.Token;
                DateTime expiryTime = _authData.ExpiryTime;
                if (!string.IsNullOrEmpty(token) && expiryTime > DateTime.UtcNow)
                    return token;
            }

            var clientId = Environment.GetEnvironmentVariable("ScheduleTasks_ClientId")
                           ?? _options.Auth.ClientId;
            var clientSecret = Environment.GetEnvironmentVariable("ScheduleTasks_ClientSecret")
                               ?? _options.Auth.ClientSecret;

            TokenResponse tokenResponse = await _client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = _options.Urls.AccountsUrl,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = "api1"
                });

            _authData = new TokenExpiryTime(tokenResponse.AccessToken, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));
            return _authData.Token;
        }
    }
}
