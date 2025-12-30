using Covenant.Common.Functionals;
using Covenant.Common.Models;

namespace Covenant.Core.BL.Interfaces
{
    public interface IPayStubService
    {
        Task<Result<ResultGenerateDocument<byte[]>>> GenerateT4(DateTime from, DateTime to);
    }
}
