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

public class DeductionImportService(
    ICraTablesContainer container,
    ICraPdfParser parser,
    IDeductionsRepository repository,
    IValidator<ImportCraTableFromBlobModel> validator,
    ILogger<DeductionImportService> logger) : IDeductionImportService
{
    public const int YearsKept = 2;

    public async Task<Result<int>> ImportCppFromBlob(ImportCraTableFromBlobModel model)
    {
        var table = await Read(model, parser.ParseCpp, CppTableValidator.Validate);
        if (table.IsFailure)
        {
            return Result.Fail<int>(table.Errors);
        }

        var deductions = table.Value
            .Select(r => new CppDeduction(r.From, r.To, r.Cpp, model.Year, model.PayPeriod))
            .ToList();
        return Imported(await repository.ImportCpp(model.Year, model.PayPeriod, deductions, YearsKept), model, "CPP");
    }

    public async Task<Result<int>> ImportTaxFromBlob(ImportCraTableFromBlobModel model)
    {
        var table = await Read(model, parser.ParseTax, TaxTableValidator.Validate);
        if (table.IsFailure)
        {
            return Result.Fail<int>(table.Errors);
        }

        var deductions = table.Value
            .Select(r => new TaxDeduction(r.From, r.To,
                r.ClaimCodes[0], r.ClaimCodes[1], r.ClaimCodes[2], r.ClaimCodes[3], r.ClaimCodes[4], r.ClaimCodes[5],
                r.ClaimCodes[6], r.ClaimCodes[7], r.ClaimCodes[8], r.ClaimCodes[9], r.ClaimCodes[10],
                model.Year, model.PayPeriod, r.TaxType))
            .ToList();
        return Imported(await repository.ImportTax(model.Year, model.PayPeriod, deductions, YearsKept), model, "income tax");
    }

    private async Task<Result<IReadOnlyList<TRow>>> Read<TRow>(
        ImportCraTableFromBlobModel model,
        Func<Stream, IReadOnlyList<TRow>> parse,
        Func<IReadOnlyList<TRow>, Result> validate)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
        {
            return Result.Fail<IReadOnlyList<TRow>>(validation.Errors.Select(e => ResultError.Create(e.PropertyName, e.ErrorMessage)));
        }

        await using var pdf = await container.DownloadStream(model.BlobName);
        if (pdf is null)
        {
            logger.LogError("The CRA table {BlobName} could not be downloaded", model.BlobName);
            return Result.Fail<IReadOnlyList<TRow>>($"{model.BlobName} was not found in the storage container");
        }

        IReadOnlyList<TRow> rows;
        try
        {
            rows = parse(pdf);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The CRA table {BlobName} could not be read", model.BlobName);
            return Result.Fail<IReadOnlyList<TRow>>($"{model.BlobName} could not be read as a PDF");
        }

        var table = validate(rows);
        if (table.IsFailure)
        {
            logger.LogError("The CRA table {BlobName} was rejected: {Errors}", model.BlobName, table.StringErrors);
            return Result.Fail<IReadOnlyList<TRow>>(table.Errors);
        }
        return Result.Ok(rows);
    }

    private Result<int> Imported(int inserted, ImportCraTableFromBlobModel model, string table)
    {
        logger.LogInformation(
            "Imported {Rows} {PayPeriod} {Table} brackets for {Year} from {BlobName}, keeping the last {YearsKept} years",
            inserted, model.PayPeriod, table, model.Year, model.BlobName, YearsKept);
        return Result.Ok(inserted);
    }
}
