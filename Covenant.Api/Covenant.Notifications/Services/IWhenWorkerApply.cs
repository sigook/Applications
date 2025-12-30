using System.Threading.Tasks;
using Covenant.Common.Args;

namespace Covenant.Notifications.Services
{
	public interface IWhenWorkerApply
	{
		Task NotifyAgency(object sender, WorkerChangeStateEventArgs args);
		Task NotifyCompany(object sender, WorkerChangeStateEventArgs args);
	}
}