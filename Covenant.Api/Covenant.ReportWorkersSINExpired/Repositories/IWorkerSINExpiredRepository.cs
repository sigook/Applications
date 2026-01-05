using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covenant.Common.Entities;
using Covenant.ReportWorkersSINExpired.Models;

namespace Covenant.ReportWorkersSINExpired.Repositories
{
	public interface IWorkerSINExpiredRepository
	{
		Task<TrackNotification> GetLastRun(string runner);
		Task<List<WorkerSINExpiredModel>> Get(DateTime now);
		Task CreateLastRun(TrackNotification entity);
		Task SaveChangesAsync();
	}
}