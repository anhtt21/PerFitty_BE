using Hnanut.PerFitty.Application.Abstractions.Auth;
using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Hnanut.PerFitty.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hnanut.PerFitty.Infrastructure.Auth;

public sealed class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public EfUserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AnyAsync(
            user => user.NormalizedEmail == normalizedEmail,
            cancellationToken);
    }

    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .Include(user => user.Profile)
            .FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .Include(user => user.Profile)
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task<RefreshToken?> FindRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}