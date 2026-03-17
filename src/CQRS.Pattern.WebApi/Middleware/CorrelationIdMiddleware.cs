using System.Diagnostics;

namespace CQRS.Pattern.WebApi.Middleware;

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;

        if (activity is not null)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Correlation-ID"] =
                    $"00-{activity.TraceId}-{activity.SpanId}-{(activity.Recorded ? "01" : "00")}";
                return Task.CompletedTask;
            });
        }

        await _next(context);
    }
}
