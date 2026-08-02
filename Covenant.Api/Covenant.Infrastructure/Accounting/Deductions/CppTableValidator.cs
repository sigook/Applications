using System.Globalization;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Deductions;

namespace Covenant.Infrastructure.Accounting.Deductions;

/// <summary>
/// Guards the import against a bad parse. A CRA table covers every earning bracket without gaps,
/// so anything else means the layout changed and the stored table must not be replaced.
/// </summary>
public static class CppTableValidator
{
    private const int MinimumRows = 1000;
    private const decimal BracketStep = 0.01m;

    public static Result Validate(IReadOnlyList<CppRow> rows)
    {
        if (rows is null || rows.Count == 0)
        {
            return Result.Fail("No CPP rows were found in the file");
        }
        if (rows.Count < MinimumRows)
        {
            return Result.Fail($"Only {rows.Count} CPP rows were found, at least {MinimumRows} are expected");
        }

        var ordered = rows.OrderBy(r => r.From).ToList();
        if (ordered[0].From != decimal.Zero)
        {
            return Result.Fail($"The first bracket starts at {Format(ordered[0].From)} instead of 0.00");
        }

        for (var index = 0; index < ordered.Count; index++)
        {
            var row = ordered[index];
            if (row.To < row.From)
            {
                return Result.Fail($"Bracket {Format(row.From)} - {Format(row.To)} ends before it starts");
            }
            if (index == 0)
            {
                continue;
            }
            var previous = ordered[index - 1];
            if (row.From != previous.To + BracketStep)
            {
                return Result.Fail($"Bracket {Format(row.From)} does not follow the previous one ending at {Format(previous.To)}");
            }
            if (row.Cpp < previous.Cpp)
            {
                return Result.Fail($"The contribution drops from {Format(previous.Cpp)} to {Format(row.Cpp)} at {Format(row.From)}");
            }
        }

        return Result.Ok();
    }

    private static string Format(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture);
}
