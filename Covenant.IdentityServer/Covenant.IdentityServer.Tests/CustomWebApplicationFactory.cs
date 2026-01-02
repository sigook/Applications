using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Covenant.IdentityServer.Tests
{
	public class CustomWebApplicationFactory<TStartup>
		: WebApplicationFactory<TStartup> where TStartup: class
	{
		protected override IWebHostBuilder CreateWebHostBuilder()
		{
			return WebHost.CreateDefaultBuilder()
				.UseWebRoot("../../../../Covenant.IdentityServer/wwwroot")
				.UseEnvironment("Development")
				.CaptureStartupErrors(true)
				.UseStartup<TStartup>();
		}
		
	}
}