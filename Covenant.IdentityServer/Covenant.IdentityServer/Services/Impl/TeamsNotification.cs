using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Covenant.IdentityServer.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Covenant.IdentityServer.Services.Impl
{
	/// <summary>
	/// This class sends notification to a channel in teams
	/// It's intended to use for development in cases where is not possible to send emails 
	/// </summary>
	public class TeamsNotification : ITeamsNotification
	{
		private readonly IHttpClientFactory _clientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<TeamsNotification> _logger;

		public TeamsNotification(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<TeamsNotification> logger)
		{
			_clientFactory = clientFactory;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task Send(TeamsNotificationModel notification)
		{
			try
			{
				var url = _configuration.GetValue<string>("TeamsIdentityServerWebhook");
				if (string.IsNullOrEmpty(url))
				{
					_logger.LogError("Teams url is empty");
					return;
				}
				string json = JsonConvert.SerializeObject(notification);
				var content = new StringContent(json);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				HttpClient client = _clientFactory.CreateClient();
				HttpResponseMessage response = await client.PostAsync(url, content);
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogError("Error sending notification {Error}", await response.Content.ReadAsStringAsync());
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error sending notification");
			}
		}
	}
}