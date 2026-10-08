using Covenant.Common.Functionals;

namespace Covenant.Core.BL.Interfaces.Shared;

public interface IUploadedFilesService
{
    T GetModel<T>();
    Result Validate();
    Task Upload(IEnumerable<string> fileNames);
}
