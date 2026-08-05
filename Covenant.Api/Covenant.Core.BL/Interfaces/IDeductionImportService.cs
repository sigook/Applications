using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Deductions;

namespace Covenant.Core.BL.Interfaces;

public interface IDeductionImportService
{
    Task<Result<int>> ImportCppFromBlob(ImportCraTableFromBlobModel model);

    Task<Result<int>> ImportTaxFromBlob(ImportCraTableFromBlobModel model);
}
