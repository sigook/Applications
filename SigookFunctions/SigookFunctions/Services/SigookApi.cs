using IdentityModel.Client;
using Newtonsoft.Json;
using SigookFunctions.Models;
using SigookFunctions.Utils;

namespace SigookFunctions.Services
{
    public class SigookApi : ISigookApi
    {
        private static readonly string SigookUrlWorkersAvailableToApply = Environment.GetEnvironmentVariable("SigookUrlWorkersAvailableToApply") ?? "https://staging.api.sigook.ca/api/Worker/AvailableToInvite";
        private static readonly HttpClient Client = new HttpClient();
        public async Task<PaginatedList<WorkerContactInfoModel>> GetWorkers(int pageIndex, Guid agencyId)
        {
            Client.SetBearerToken(await Client.GetToken());
            HttpResponseMessage response = await Client.GetAsync($"{SigookUrlWorkersAvailableToApply}?PageSize=100&PageIndex={pageIndex}&AgencyId={agencyId}");
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PaginatedList<WorkerContactInfoModel>>(content);
        }
    }
}