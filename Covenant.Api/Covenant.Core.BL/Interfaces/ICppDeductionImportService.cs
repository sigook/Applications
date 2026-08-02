using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Deductions;

namespace Covenant.Core.BL.Interfaces;

public interface ICppDeductionImportService
{
    Task<Result<int>> ImportFromBlob(ImportCppFromBlobModel model);
}
