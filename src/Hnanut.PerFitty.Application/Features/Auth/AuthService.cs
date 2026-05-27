using Hnanut.PerFitty.Application.Abstractions;
using Hnanut.PerFitty.Application.Abstractions.Auth;
using Hnanut.PerFitty.Domain.Modules.Auth.Entities;

namespace Hnanut.PerFitty.Application.Features.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthTokenService _tokenService;
    private readonly IClock _clock;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IAuthTokenService tokenService,
        IClock clock)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _clock = clock;
    }

    public async Task<AuthResult<AuthTokenResponse>> RegisterAsync(
        RegisterRequest request,
        AuthClientContext clientContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return AuthResult<AuthTokenResponse>.Failure("invalid_register_request", "Email and password are required.");
        }

        if (request.Password.Length < 8)
        {
            return AuthResult<AuthTokenResponse>.Failure("weak_password", "Password must be at least 8 characters.");
        }

        var normalizedEmail = NormalizeEmail(request.Email);
        if (await _users.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return AuthResult<AuthTokenResponse>.Failure("email_already_exists", "Email is already registered.");
        }

        var now = _clock.UtcNow;
        var user = User.Create(request.Email, _passwordHasher.Hash(request.Password), request.DisplayName);
        var accessToken = _tokenService.CreateAccessToken(user, now);
        var refreshSecret = _tokenService.CreateRefreshToken(now);

        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshSecret.Hash,
            refreshSecret.ExpiresAt,
            clientContext.DeviceId,
            clientContext.DeviceName,
            clientContext.IpAddress,
            clientContext.UserAgent,
            refreshSecret.TokenFamilyId);

        await _users.AddAsync(user, cancellationToken);
        await _users.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return AuthResult<AuthTokenResponse>.Success(CreateResponse(user, accessToken, refreshSecret));
    }

    public async Task<AuthResult<AuthTokenResponse>> LoginAsync(
        LoginRequest request,
        AuthClientContext clientContext,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await _users.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return AuthResult<AuthTokenResponse>.Failure("invalid_credentials", "Email or password is incorrect.");
        }

        if (user.IsDisabled)
        {
            return AuthResult<AuthTokenResponse>.Failure("account_disabled", "This account is disabled.");
        }

        var now = _clock.UtcNow;
        user.MarkLogin(now);

        var accessToken = _tokenService.CreateAccessToken(user, now);
        var refreshSecret = _tokenService.CreateRefreshToken(now);

        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshSecret.Hash,
            refreshSecret.ExpiresAt,
            clientContext.DeviceId,
            clientContext.DeviceName,
            clientContext.IpAddress,
            clientContext.UserAgent,
            refreshSecret.TokenFamilyId);

        await _users.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return AuthResult<AuthTokenResponse>.Success(CreateResponse(user, accessToken, refreshSecret));
    }

    public async Task<AuthResult<AuthTokenResponse>> RefreshAsync(
        RefreshTokenRequest request,
        AuthClientContext clientContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return AuthResult<AuthTokenResponse>.Failure("invalid_refresh_token", "Refresh token is required.");
        }

        var now = _clock.UtcNow;
        var oldTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var oldToken = await _users.FindRefreshTokenByHashAsync(oldTokenHash, cancellationToken);

        if (oldToken is null || !oldToken.IsActive(now))
        {
            return AuthResult<AuthTokenResponse>.Failure("invalid_refresh_token", "Refresh token is invalid or expired.");
        }

        var user = await _users.FindByIdAsync(oldToken.UserId, cancellationToken);
        if (user is null || user.IsDisabled)
        {
            return AuthResult<AuthTokenResponse>.Failure("invalid_refresh_token", "Refresh token is invalid.");
        }

        var accessToken = _tokenService.CreateAccessToken(user, now);
        var refreshSecret = _tokenService.CreateRefreshToken(now, oldToken.TokenFamilyId);

        oldToken.Revoke(now, refreshSecret.Hash);

        var newRefreshToken = RefreshToken.Create(
            user.Id,
            refreshSecret.Hash,
            refreshSecret.ExpiresAt,
            clientContext.DeviceId,
            clientContext.DeviceName,
            clientContext.IpAddress,
            clientContext.UserAgent,
            refreshSecret.TokenFamilyId);

        await _users.AddRefreshTokenAsync(newRefreshToken, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return AuthResult<AuthTokenResponse>.Success(CreateResponse(user, accessToken, refreshSecret));
    }

    public async Task<AuthResult<LogoutResponse>> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var now = _clock.UtcNow;
            var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
            var token = await _users.FindRefreshTokenByHashAsync(tokenHash, cancellationToken);

            if (token is not null && !token.IsRevoked)
            {
                token.Revoke(now);
                await _users.SaveChangesAsync(cancellationToken);
            }
        }

        return AuthResult<LogoutResponse>.Success(new LogoutResponse(true));
    }

    public async Task<AuthResult<CurrentUserResponse>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _users.FindByIdAsync(userId, cancellationToken);

        return user is null
            ? AuthResult<CurrentUserResponse>.Failure("user_not_found", "User was not found.")
            : AuthResult<CurrentUserResponse>.Success(AuthResponseMapper.ToCurrentUserResponse(user));
    }

    private static AuthTokenResponse CreateResponse(
        User user,
        AccessTokenSecret accessToken,
        RefreshTokenSecret refreshToken)
    {
        return new AuthTokenResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            AuthResponseMapper.ToAuthUserResponse(user));
    }

    private static string NormalizeEmail(string email)
        => email.Trim().ToUpperInvariant();
}