namespace Hnanut.PerFitty.SharedKernel.Api;

public sealed record ErrorResponse(
    string Code,
    string Message,
    string? TraceId = null,
    string? CorrelationId = null,
    IReadOnlyDictionary<string, string[]>? Details = null)
{
    public static ErrorResponse Unexpected(string? traceId, string? correlationId)
        => new(
            "unexpected_error",
            "An unexpected error occurred.",
            traceId,
            correlationId);
}
