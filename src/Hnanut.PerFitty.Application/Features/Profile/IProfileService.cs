namespace Hnanut.PerFitty.Application.Features.Profile;

public interface IProfileService
{
    Task<ProfileResult<ProfileResponse>> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<ProfileResult<ProfileResponse>> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken);

    Task<ProfileResult<StylePreferencesResponse>> GetStylePreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<ProfileResult<StylePreferencesResponse>> UpdateStylePreferencesAsync(
        Guid userId,
        UpdateStylePreferencesRequest request,
        CancellationToken cancellationToken);
}