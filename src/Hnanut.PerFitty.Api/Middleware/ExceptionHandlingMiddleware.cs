using System.Diagnostics;
using Hnanut.PerFitty.SharedKernel.Api;
using Microsoft.AspNetCore.Mvc;

namespace Hnanut.PerFitty.Api.Middleware;

public sealed partial class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            LogUnhandledApiException(_logger, exception);

            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            var correlationId = context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value)
                ? value?.ToString()
                : null;

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected server error",
                Detail = "An unexpected error occurred while processing the request.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;
            problem.Extensions["correlationId"] = correlationId;
            problem.Extensions["error"] = ErrorResponse.Unexpected(traceId, correlationId);

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Unhandled API exception")]
    private static partial void LogUnhandledApiException(ILogger logger, Exception exception);
}
