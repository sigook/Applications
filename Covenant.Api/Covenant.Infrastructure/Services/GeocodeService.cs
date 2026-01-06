using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Covenant.Infrastructure.Services
{
    public class GeocodeService : IGeocodeService
    {
        private readonly ILocationRepository locationRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<GeocodeService> logger;
        private readonly GeocodeGoogleConfiguration configuration;

        public GeocodeService(
            ILocationRepository locationRepository,
            IOptions<GeocodeGoogleConfiguration> options,
            IHttpClientFactory httpClientFactory,
            ILogger<GeocodeService> logger)
        {
            this.locationRepository = locationRepository;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            configuration = options.Value;
        }

        public async Task<GeocodeGeometryLocation> GetLocationGeocode(string address)
        {
            try
            {
                address = address.Replace("#", string.Empty);
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{configuration.Url}?address={address}&key={configuration.Key}");
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GeocodeResponse>(content);
                if (result.Status.Equals("OK", StringComparison.InvariantCultureIgnoreCase) && result.Results.Any())
                {
                    return result.Results[0].Geometry.Location;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting location info for: {Address}", address);
            }
            return new GeocodeGeometryLocation
            {
                Lat = Location.DefaultLatitude,
                Lng = Location.DefaultLongitude
            };
        }
    }
}
