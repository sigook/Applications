namespace Covenant.Api.Middlewares;

public class BufferingMiddleware
{
    private readonly RequestDelegate next;

    public BufferingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        await next(context);
        context.Request.Body.Position = 0;
    }
}
