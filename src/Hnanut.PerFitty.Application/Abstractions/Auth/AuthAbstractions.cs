using Hnanut.PerFitty.Domain.Modules.Auth.Entities;

namespace Hnanut.PerFitty.Application.Abstractions.Auth;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface IAuthTokenService
{
    AccessTokenSecret CreateAccessToken(User user, DateTimeOffset now);
    RefreshTokenSecret CreateRefreshToken(DateTimeOffset now, Guid? tokenFamilyId = null);
    string HashRefreshToken(string refreshToken);
}

public sealed record AccessTokenSecret(string Token, DateTimeOffset ExpiresAt);

public sealed record RefreshTokenSecret(
    string Token,
    string Hash,
    Guid TokenFamilyId,
    DateTimeOffset ExpiresAt);