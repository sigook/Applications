using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Accounting;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Covenant.Infrastructure.Accounting.Deductions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Covenant.Core.BL.Services.Accounting;

public class CppDeductionImportService(
    ICraTablesContainer container,
    ICppPdfParser parser,
    IDeductionsRepository repository,
    IValidator<ImportCppFromBlobModel> validator,
    ILogger<CppDeductionImportService> logger) : ICppDeductionImportService
{
    public async Task<Result<int>> ImportFromBlob(ImportCppFromBlobModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
        {
            return Result.Fail<int>(validation.Errors.Select(e => ResultError.Create(e.PropertyName, e.ErrorMessage)));
        }

        await using var pdf = await container.DownloadStream(model.BlobName);
        if (pdf is null)
        {
            logger.LogError("The CRA table {BlobName} could not be downloaded", model.BlobName);
            return Result.Fail<int>($"{model.BlobName} was not found in the storage container");
        }

        IReadOnlyList<CppRow> rows;
        try
        {
            rows = parser.Parse(pdf);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The CRA table {BlobName} could not be read", model.BlobName);
            return Result.Fail<int>($"{model.BlobName} could not be read as a PDF");
        }

        var table = CppTableValidator.Validate(rows);
        if (table.IsFailure)
        {
            logger.LogError("The CRA table {BlobName} was rejected: {Errors}", model.BlobName, table.StringErrors);
            return Result.Fail<int>(table.Errors);
        }

        var deductions = rows.Select(r => new CppDeduction(r.From, r.To, r.Cpp, model.Year, model.PayPeriod)).ToList();
        var inserted = await repository.ReplaceCpp(model.Year, model.PayPeriod, deductions);
        logger.LogInformation("Imported {Rows} {PayPeriod} CPP brackets for {Year} from {BlobName}",
            inserted, model.PayPeriod, model.Year, model.BlobName);
        return Result.Ok(inserted);
    }
}
