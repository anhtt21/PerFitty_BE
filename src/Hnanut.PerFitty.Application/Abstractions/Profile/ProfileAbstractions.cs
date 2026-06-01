using Hnanut.PerFitty.Domain.Modules.Auth.Entities;

namespace Hnanut.PerFitty.Application.Abstractions.Profile;

public interface IProfileRepository
{
    Task<UserProfile?> FindProfileByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<UserStylePreference?> FindStylePreferenceByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task AddStylePreferenceAsync(
        UserStylePreference preference,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}