using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Sigook.Functions.Models;

namespace Sigook.Functions.Utils
{
    public static class AccessToken
    {
        private static TokenExpiryTime _authData;

        public static async Task<string> GetToken(this HttpClient client, IConfiguration configuration)
        {
            if (_authData != null)
            {
                string token = _authData.Token;
                DateTime expiryTime = _authData.ExpiryTime;
                if (!string.IsNullOrEmpty(token) && expiryTime > DateTime.UtcNow) return token;
            }
            TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = configuration["ScheduleTasks_AccountsUrl"],
                    ClientId = configuration["ScheduleTasks_ClientId"],
                    ClientSecret = configuration["ScheduleTasks_ClientSecret"],
                    Scope = "api1"
                });
            _authData = new TokenExpiryTime(tokenResponse.AccessToken, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));
            var tokenExpiryTime = _authData;
            return tokenExpiryTime.Token;
        }
    }
}
