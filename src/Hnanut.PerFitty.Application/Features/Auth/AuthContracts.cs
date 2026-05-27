using Hnanut.PerFitty.Domain.Modules.Auth.Entities;

namespace Hnanut.PerFitty.Application.Features.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string? DisplayName,
    string? DeviceId,
    string? DeviceName);

public sealed record LoginRequest(
    string Email,
    string Password,
    string? DeviceId,
    string? DeviceName);

public sealed record RefreshTokenRequest(
    string RefreshToken,
    string? DeviceId,
    string? DeviceName);

public sealed record LogoutRequest(string RefreshToken);

public sealed record AuthClientContext(
    string? DeviceId,
    string? DeviceName,
    string? IpAddress,
    string? UserAgent);

public sealed record AuthTokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    AuthUserResponse User);

public sealed record AuthUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    bool EmailConfirmed);

public sealed record CurrentUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    bool EmailConfirmed);

public sealed record LogoutResponse(bool Succeeded);

public sealed record AuthFailure(string Code, string Message);

public sealed class AuthResult<T>
{
    internal AuthResult(T? value, AuthFailure? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }

    public AuthFailure? Error { get; }

    public bool Succeeded => Error is null;
}

public static class AuthResult
{
    public static AuthResult<T> Success<T>(T value) => new(value, null);

    public static AuthResult<T> Failure<T>(string code, string message)
        => new(default, new AuthFailure(code, message));
}

public static class AuthResponseMapper
{
    public static AuthUserResponse ToAuthUserResponse(User user)
        => new(
            user.Id,
            user.Email,
            user.Profile?.DisplayName ?? user.Email,
            user.EmailConfirmed);

    public static CurrentUserResponse ToCurrentUserResponse(User user)
        => new(
            user.Id,
            user.Email,
            user.Profile?.DisplayName ?? user.Email,
            user.EmailConfirmed);
}
