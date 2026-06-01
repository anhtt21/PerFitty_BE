using Hnanut.PerFitty.Application.Abstractions.Profile;
using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Hnanut.PerFitty.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hnanut.PerFitty.Infrastructure.Auth;

public sealed class EfProfileRepository : IProfileRepository
{
    private readonly AppDbContext _dbContext;

    public EfProfileRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserProfile?> FindProfileByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.UserProfiles
            .FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public Task<UserStylePreference?> FindStylePreferenceByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _dbContext.UserStylePreferences
            .Include(preference => preference.Values)
            .FirstOrDefaultAsync(preference => preference.UserId == userId, cancellationToken);
    }

    public async Task AddStylePreferenceAsync(
        UserStylePreference preference,
        CancellationToken cancellationToken)
    {
        await _dbContext.UserStylePreferences.AddAsync(preference, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}