using System.Threading.Tasks;
using Covenant.IdentityServer.Services;
using Covenant.IdentityServer.Views.Notifications;

namespace Covenant.IdentityServer.Tests.Utils
{
	internal class FakeRazorViewToStringRenderer: IRazorViewToStringRenderer
	{
		public ConfirmAccountViewModel ConfirmAccountViewModel { get; set; }
		public Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
		{
			ConfirmAccountViewModel= model as ConfirmAccountViewModel;
			return Task.FromResult(viewName);
		}
	}
}