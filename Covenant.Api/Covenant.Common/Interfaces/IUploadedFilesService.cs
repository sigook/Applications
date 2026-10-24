using Covenant.Common.Functionals;

namespace Covenant.Common.Interfaces;

public interface IUploadedFilesService
{
    T GetModel<T>();
    IReadOnlyList<string> GetFileNames();
    Result Validate();
    Task Upload(IEnumerable<string> fileNames);
}
