using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Location;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Covenant.Infrastructure.Services;

public class GeocodeService : IGeocodeService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<GeocodeService> logger;
    private readonly GeocodeGoogleConfiguration configuration;

    public GeocodeService(
        IOptions<GeocodeGoogleConfiguration> options,
        IHttpClientFactory httpClientFactory,
        ILogger<GeocodeService> logger)
    {
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
            var content = await response.Content.ReadFromJsonAsync<GeocodeResponse>();
            if (content.Status.Equals("OK", StringComparison.InvariantCultureIgnoreCase) && content.Results.Any())
            {
                return content.Results[0].Geometry.Location;
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
