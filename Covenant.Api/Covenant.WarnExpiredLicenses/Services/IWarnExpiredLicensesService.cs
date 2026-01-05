using System;
using System.Threading.Tasks;
using Covenant.Common.Args;

namespace Covenant.WarnExpiredLicenses.Services
{
	public interface IWarnExpiredLicensesService
	{
		event Func<object, WorkerLicenseExpiredEventArgs, Task> OnAgencyToNotify;
		Task Warn();
	}
}