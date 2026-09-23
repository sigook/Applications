using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Covenant.Api.Authorization;

public class SecurityHeadersAttribute : ActionFilterAttribute
{
    private const string ContentSecurityPolicy =
        "default-src 'self' http://www.w3.org/2000/svg http://www.w3.org/1999/xlink https://www.google.com; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';"
        + " script-src 'self' https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/ 'unsafe-inline' 'unsafe-eval';";

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is not ViewResult) return;

        var headers = context.HttpContext.Response.Headers;
        headers.TryAdd("X-Content-Type-Options", "nosniff");
        headers.TryAdd("X-Frame-Options", "SAMEORIGIN");
        headers.TryAdd("Content-Security-Policy", ContentSecurityPolicy);
        headers.TryAdd("X-Content-Security-Policy", ContentSecurityPolicy);
        headers.TryAdd("Referrer-Policy", "no-referrer");
    }
}
