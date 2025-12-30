using Covenant.Common.Functionals;
using FluentValidation.Results;

namespace Covenant.Common.Utils.Extensions;

public static class FluentValidationExtensions
{
    public static Result ToResultFailure(this ValidationResult result)
    {
        return Result.Fail(result.Errors.Select(e => new ResultError(e.PropertyName, e.ErrorMessage)));
    }

    public static Result<T> ToResultFailure<T>(this ValidationResult result)
    {
        return Result.Fail<T>(result.Errors.Select(e => new ResultError(e.PropertyName, e.ErrorMessage)));
    }
}
