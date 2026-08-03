using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Accounting;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
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

    private const int MinimumCppBrackets = 1000;
    private const int MinimumTaxBrackets = 200;
    private const decimal CppBracketStep = 0.01m;

    public async Task<Result<int>> ImportCppFromBlob(ImportCraTableFromBlobModel model)
    {
        var table = await Read(model, parser.ParseCpp, ValidateCpp);
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
        var table = await Read(model, parser.ParseTax, ValidateTax);
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

    /// <summary>
    /// Guards the import against a bad parse. A CRA CPP table covers every earning bracket without gaps,
    /// so anything else means the layout changed and the stored table must not be replaced.
    /// </summary>
    private static Result ValidateCpp(IReadOnlyList<CppRow> rows)
    {
        if (rows is null || rows.Count == 0)
        {
            return Result.Fail("No CPP rows were found in the file");
        }
        if (rows.Count < MinimumCppBrackets)
        {
            return Result.Fail($"Only {rows.Count} CPP rows were found, at least {MinimumCppBrackets} are expected");
        }

        var ordered = rows.OrderBy(r => r.From).ToList();
        if (ordered[0].From != decimal.Zero)
        {
            return Result.Fail($"The first bracket starts at {ordered[0].From.ToAmount()} instead of 0.00");
        }

        for (var index = 0; index < ordered.Count; index++)
        {
            var row = ordered[index];
            if (row.To < row.From)
            {
                return Result.Fail($"Bracket {row.From.ToAmount()} - {row.To.ToAmount()} ends before it starts");
            }
            if (index == 0)
            {
                continue;
            }
            var previous = ordered[index - 1];
            if (row.From != previous.To + CppBracketStep)
            {
                return Result.Fail($"Bracket {row.From.ToAmount()} does not follow the previous one ending at {previous.To.ToAmount()}");
            }
            if (row.Cpp < previous.Cpp)
            {
                return Result.Fail($"The contribution drops from {previous.Cpp.ToAmount()} to {row.Cpp.ToAmount()} at {row.From.ToAmount()}");
            }
        }

        return Result.Ok();
    }

    /// <summary>
    /// Guards the import against a bad parse. A CRA income tax file holds a federal and a provincial table,
    /// each one covering every earning bracket without gaps and each claim code growing with the earnings
    /// once the first amount shows up, so anything else means the layout changed and the stored table must not be replaced.
    /// </summary>
    private static Result ValidateTax(IReadOnlyList<TaxRow> rows)
    {
        if (rows is null || rows.Count == 0)
        {
            return Result.Fail("No tax rows were found in the file");
        }

        foreach (var taxType in Enum.GetValues<TaxType>())
        {
            var table = rows.Where(r => r.TaxType == taxType).OrderBy(r => r.From).ToList();
            var result = ValidateTaxTable(taxType, table);
            if (result.IsFailure)
            {
                return result;
            }
        }
        return Result.Ok();
    }

    private static Result ValidateTaxTable(TaxType taxType, IReadOnlyList<TaxRow> rows)
    {
        if (rows.Count < MinimumTaxBrackets)
        {
            return Result.Fail($"Only {rows.Count} {taxType} brackets were found, at least {MinimumTaxBrackets} are expected");
        }
        if (rows[0].From != decimal.Zero)
        {
            return Result.Fail($"The first {taxType} bracket starts at {rows[0].From.ToAmount()} instead of 0.00");
        }

        for (var index = 0; index < rows.Count; index++)
        {
            var row = rows[index];
            if (row.To <= row.From)
            {
                return Result.Fail($"The {taxType} bracket {row.From.ToAmount()} - {row.To.ToAmount()} ends before it starts");
            }
            if (row.ClaimCodes.Count != TaxRow.ClaimCodeCount)
            {
                return Result.Fail($"The {taxType} bracket {row.From.ToAmount()} holds {row.ClaimCodes.Count} claim codes instead of {TaxRow.ClaimCodeCount}");
            }
            if (row.ClaimCodes.All(c => c is null))
            {
                return Result.Fail($"The {taxType} bracket {row.From.ToAmount()} - {row.To.ToAmount()} holds no amount at all");
            }
            if (index > 0 && row.From != rows[index - 1].To)
            {
                return Result.Fail($"The {taxType} bracket {row.From.ToAmount()} does not follow the previous one ending at {rows[index - 1].To.ToAmount()}");
            }
        }

        for (var claimCode = 0; claimCode < TaxRow.ClaimCodeCount; claimCode++)
        {
            var result = ValidateClaimCode(taxType, rows, claimCode);
            if (result.IsFailure)
            {
                return result;
            }
        }
        return Result.Ok();
    }

    private static Result ValidateClaimCode(TaxType taxType, IReadOnlyList<TaxRow> rows, int claimCode)
    {
        decimal? previous = null;
        foreach (var row in rows)
        {
            var amount = row.ClaimCodes[claimCode];
            if (amount is null)
            {
                if (previous is not null)
                {
                    return Result.Fail($"The {taxType} claim code {claimCode} loses its amount at {row.From.ToAmount()} after {previous.Value.ToAmount()}");
                }
                continue;
            }
            if (previous is not null && amount < previous)
            {
                return Result.Fail($"The {taxType} claim code {claimCode} drops from {previous.Value.ToAmount()} to {amount.Value.ToAmount()} at {row.From.ToAmount()}");
            }
            previous = amount;
        }
        return previous is null
            ? Result.Fail($"The {taxType} claim code {claimCode} holds no amount at all")
            : Result.Ok();
    }
}
