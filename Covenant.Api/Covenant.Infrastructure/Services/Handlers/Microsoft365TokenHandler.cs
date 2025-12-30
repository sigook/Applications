using Azure.Core;
using Azure.Identity;
using Covenant.Common.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace Covenant.Infrastructure.Services.Handlers
{
    public class Microsoft365TokenHandler : DelegatingHandler
    {
        private readonly Microsoft365Configuration microsoft365Configuration;
        private readonly ILogger<Microsoft365TokenHandler> logger;

        public Microsoft365TokenHandler(
            IOptions<Microsoft365Configuration> options,
            ILogger<Microsoft365TokenHandler> logger)
        {
            microsoft365Configuration = options.Value;
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }

        public async Task<string> GetToken()
        {
            var token = string.Empty;
            try
            {
                var credentials = new ClientSecretCredential(microsoft365Configuration.TenantId, microsoft365Configuration.ClientId, microsoft365Configuration.ClientSecret);
                var scopes = new string[] { microsoft365Configuration.Scope };
                var accessToken = await credentials.GetTokenAsync(new TokenRequestContext(scopes));
                token = accessToken.Token;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting token from microsoft 365");
            }
            return token;
        }
    }
}
