using System;
using System.Threading.Tasks;
using Covenant.Common.Args;

namespace Covenant.ReportWorkersSINExpired.Services
{
	public interface IReportWorkersSINExpiredService
	{
		event Func<object, WorkerSINExpiredEventArgs,Task> OnAgencyToNotify;
		Task Report();
	}
}