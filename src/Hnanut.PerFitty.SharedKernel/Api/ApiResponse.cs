namespace Hnanut.PerFitty.SharedKernel.Api;

public sealed record ApiResponse<T>(
    bool Succeeded,
    T? Data,
    ErrorResponse? Error);

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data)
        => new(true, data, null);

    public static ApiResponse<T> Failure<T>(ErrorResponse error)
        => new(false, default, error);
}
