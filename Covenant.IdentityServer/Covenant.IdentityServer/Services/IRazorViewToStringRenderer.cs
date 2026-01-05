using System.Threading.Tasks;

namespace Covenant.IdentityServer.Services
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}