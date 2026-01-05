using Covenant.Common.Functionals;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Covenant.Api.Utils.Extensions
{
    public static class ModelStateExtensions
    {
        public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, IEnumerable<ResultError> errors)
        {
            foreach (ResultError error in errors)
            {
                modelState.AddModelError(error.Key, error.Message);
            }
            return modelState;
        }

        public static ModelStateDictionary AddError(this ModelStateDictionary modelState, string error, string key = null)
        {
            modelState.AddModelError(key ?? string.Empty, error);
            return modelState;
        }
    }
}