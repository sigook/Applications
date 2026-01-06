using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Utils.Extensions
{
    public static class ControllerResult
    {   
        public static IActionResult GetByIdResult<T>(this ControllerBase controller, T detail) where T : class
        {
            if (detail is null) return controller.NotFound();
            return controller.Ok(detail);
        }
    }
}