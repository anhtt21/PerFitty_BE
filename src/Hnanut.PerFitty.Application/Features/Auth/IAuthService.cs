namespace Hnanut.PerFitty.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResult<AuthTokenResponse>> RegisterAsync(
        RegisterRequest request,
        AuthClientContext clientContext,
        CancellationToken cancellationToken);

    Task<AuthResult<AuthTokenResponse>> LoginAsync(
        LoginRequest request,
        AuthClientContext clientContext,
        CancellationToken cancellationToken);

    Task<AuthResult<AuthTokenResponse>> RefreshAsync(
        RefreshTokenRequest request,
        AuthClientContext clientContext,
        CancellationToken cancellationToken);

    Task<AuthResult<LogoutResponse>> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken);

    Task<AuthResult<CurrentUserResponse>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken);
}