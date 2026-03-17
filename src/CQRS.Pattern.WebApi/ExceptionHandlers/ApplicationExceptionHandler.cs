using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CQRS.Pattern.Application.Common.Exceptions;

namespace CQRS.Pattern.WebApi.ExceptionHandlers;

public sealed class ApplicationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApplicationExceptionHandler> _logger;

    public ApplicationExceptionHandler(ILogger<ApplicationExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not IHasHttpStatus statusException)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusException.StatusCode,
            Title = statusException.Title,
            Detail = exception.Message,
            Type = statusException.Type,
        };

        if (statusException.Extensions is { } extensions)
        {
            foreach (var (key, value) in extensions)
            {
                problemDetails.Extensions[key] = value;
            }
        }

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.RequestServices
            .GetRequiredService<IProblemDetailsService>()
            .WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception,
            });
        return true;
    }
}
