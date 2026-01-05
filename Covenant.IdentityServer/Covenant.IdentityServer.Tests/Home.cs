using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Covenant.IdentityServer.Tests
{
	public class Home : IClassFixture<CustomWebApplicationFactory<Startup>>
	{
		private readonly CustomWebApplicationFactory<Startup> _factory;
		private readonly HttpClient _client;

		public Home(CustomWebApplicationFactory<Startup> factory)
		{
			_factory = factory;
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task Get()
		{
			HttpResponseMessage response = await _client.GetAsync("/");
			response.EnsureSuccessStatusCode();
		}
	}
}