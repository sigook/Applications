using SigookFunctions.Models;
using SigookFunctions.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
	public class SigookApiTest
	{
		[Fact]
		public async Task GetWorkers()
		{
			Environment.SetEnvironmentVariable("ScheduleTasks_AccountsUrl", "https://staging.accounts.sigook.ca/connect/token");
			Environment.SetEnvironmentVariable("ScheduleTasks_ClientId", "schedule.service");
			Environment.SetEnvironmentVariable("ScheduleTasks_ClientSecret", "EC26EC05-526D-4B12-A494-496464840AAA");
			var sigookApi = new SigookApi();
			PaginatedList<WorkerContactInfoModel> list = await sigookApi.GetWorkers(1, Guid.Parse("9bcbfae3-5784-46a0-8eb0-f3fab5798746"));
			Assert.NotEmpty(list.Items);
		}
	}
}