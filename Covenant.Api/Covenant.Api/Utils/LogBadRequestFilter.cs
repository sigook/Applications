using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net;

namespace Covenant.Api.Utils
{
    public class LogBadRequestFilter : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode != (int)HttpStatusCode.BadRequest) return;
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<LogBadRequestFilter>>();
            string message = string.Concat(context.ModelState.SelectMany(c => c.Value.Errors.Select(e => $" {c.Key} {e.ErrorMessage} ")));
            logger.LogWarning($"**********************************{message}");
            Debug.WriteLine(message);
        }
    }
}