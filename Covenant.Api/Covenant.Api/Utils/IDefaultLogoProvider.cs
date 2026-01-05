using Covenant.Common.Models;

namespace Covenant.Api.Utils
{
    public interface IDefaultLogoProvider
    {
        Task<CovenantFileModel> GetLogo(string name);
    }
}