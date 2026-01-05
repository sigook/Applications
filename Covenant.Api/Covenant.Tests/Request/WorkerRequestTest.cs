using System;
using Xunit;

namespace Covenant.Tests.Request
{
	public class WorkerRequestTest
	{
		[Fact]
		public void UpdateStartWorking()
		{
			var workerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(Guid.NewGuid(), Guid.NewGuid());
			var startWorking = new DateTime(2021,04,13);
			var weekStartWorking = new DateTime(2021,04,11);
			workerRequest.UpdateStartWorking(startWorking);
			Assert.Equal(workerRequest.StartWorking,startWorking);
			Assert.Equal(weekStartWorking,workerRequest.WeekStartWorking);
			startWorking = startWorking.AddDays(1);
			workerRequest.UpdateStartWorking(startWorking);
			Assert.Equal(workerRequest.StartWorking,startWorking);
			Assert.Equal(weekStartWorking,workerRequest.WeekStartWorking);
		}
	}
}