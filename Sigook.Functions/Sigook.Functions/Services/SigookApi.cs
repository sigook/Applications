using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sigook.Functions.Models;
using Sigook.Functions.Utils;
using System.Net.Http.Headers;

namespace Sigook.Functions.Services
{
    public class SigookApi : ISigookApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SigookApi(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<PaginatedList<WorkerContactInfoModel>> GetWorkers(int pageIndex, Guid agencyId)
        {
            var baseUrl = _configuration["SigookUrlWorkersAvailableToApply"]
                ?? "https://staging.api.sigook.ca/api/Worker/AvailableToInvite";

            var client = _httpClientFactory.CreateClient("Api");
            var token = await client.GetToken(_configuration);

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{baseUrl}?PageSize=100&PageIndex={pageIndex}&AgencyId={agencyId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PaginatedList<WorkerContactInfoModel>>(content);
        }
    }
}
