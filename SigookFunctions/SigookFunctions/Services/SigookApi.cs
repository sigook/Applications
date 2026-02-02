using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SigookFunctions.Models;
using SigookFunctions.Options;

namespace SigookFunctions.Services
{
    public class SigookApi : ISigookApi
    {
        private readonly SigookFunctionsOptions _options;
        private readonly ITokenService _tokenService;
        private readonly HttpClient _client;

        public SigookApi(IOptions<SigookFunctionsOptions> options, ITokenService tokenService)
        {
            _options = options.Value;
            _tokenService = tokenService;
            _client = new HttpClient();
        }

        public async Task<PaginatedList<WorkerContactInfoModel>> GetWorkers(int pageIndex, Guid agencyId)
        {
            _client.SetBearerToken(await _tokenService.GetTokenAsync());

            HttpResponseMessage response = await _client.GetAsync(
                $"{_options.Urls.WorkersAvailableToApply}?PageSize=100&PageIndex={pageIndex}&AgencyId={agencyId}");

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PaginatedList<WorkerContactInfoModel>>(content);
        }
    }
}
