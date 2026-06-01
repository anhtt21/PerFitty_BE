using Hnanut.PerFitty.Application.Abstractions;
using Hnanut.PerFitty.Application.Abstractions.Profile;
using Hnanut.PerFitty.Domain.Modules.Auth.Entities;

namespace Hnanut.PerFitty.Application.Features.Profile;

public sealed class ProfileService : IProfileService
{
    private static readonly HashSet<string> AllowedStyles = new(StringComparer.OrdinalIgnoreCase)
    {
        "basic",
        "minimalist",
        "korean",
        "streetwear",
        "vintage",
        "feminine",
        "formal",
        "sporty",
        "y2k",
        "casual"
    };

    private static readonly HashSet<string> AllowedOccasions = new(StringComparer.OrdinalIgnoreCase)
    {
        "school",
        "work",
        "hangout",
        "date",
        "interview",
        "travel",
        "party",
        "home"
    };

    private readonly IProfileRepository _profiles;
    private readonly IClock _clock;

    public ProfileService(IProfileRepository profiles, IClock clock)
    {
        _profiles = profiles;
        _clock = clock;
    }

    public async Task<ProfileResult<ProfileResponse>> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var profile = await _profiles.FindProfileByUserIdAsync(userId, cancellationToken);

        return profile is null
            ? ProfileResult.Failure<ProfileResponse>("profile_not_found", "Profile was not found.")
            : ProfileResult.Success(ToProfileResponse(profile));
    }

    public async Task<ProfileResult<ProfileResponse>> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return ProfileResult.Failure<ProfileResponse>("invalid_profile_request", "Display name is required.");
        }

        if (request.HeightCm is < 80 or > 250)
        {
            return ProfileResult.Failure<ProfileResponse>("invalid_profile_request", "Height must be between 80 and 250 cm.");
        }

        var profile = await _profiles.FindProfileByUserIdAsync(userId, cancellationToken);
        if (profile is null)
        {
            return ProfileResult.Failure<ProfileResponse>("profile_not_found", "Profile was not found.");
        }

        profile.UpdateBasicInfo(
            request.DisplayName,
            EmptyToNull(request.AvatarObjectKey),
            EmptyToNull(request.Gender),
            request.HeightCm,
            EmptyToNull(request.BodyShape),
            _clock.UtcNow);

        await _profiles.SaveChangesAsync(cancellationToken);

        return ProfileResult.Success(ToProfileResponse(profile));
    }

    public async Task<ProfileResult<StylePreferencesResponse>> GetStylePreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var preference = await _profiles.FindStylePreferenceByUserIdAsync(userId, cancellationToken);

        return ProfileResult.Success(ToStylePreferencesResponse(preference));
    }

    public async Task<ProfileResult<StylePreferencesResponse>> UpdateStylePreferencesAsync(
        Guid userId,
        UpdateStylePreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var preferredStyles = Normalize(request.PreferredStyles);
        var preferredOccasions = Normalize(request.PreferredOccasions);
        var favoriteColors = Normalize(request.FavoriteColors);
        var avoidedColors = Normalize(request.AvoidedColors);

        var invalidStyle = preferredStyles.FirstOrDefault(style => !AllowedStyles.Contains(style));
        if (invalidStyle is not null)
        {
            return ProfileResult.Failure<StylePreferencesResponse>(
                "invalid_style_preference",
                $"Style '{invalidStyle}' is not supported.");
        }

        var invalidOccasion = preferredOccasions.FirstOrDefault(occasion => !AllowedOccasions.Contains(occasion));
        if (invalidOccasion is not null)
        {
            return ProfileResult.Failure<StylePreferencesResponse>(
                "invalid_style_preference",
                $"Occasion '{invalidOccasion}' is not supported.");
        }

        var preference = await _profiles.FindStylePreferenceByUserIdAsync(userId, cancellationToken);
        if (preference is null)
        {
            preference = UserStylePreference.Create(
                userId,
                preferredStyles,
                preferredOccasions,
                favoriteColors,
                avoidedColors,
                _clock.UtcNow);

            await _profiles.AddStylePreferenceAsync(preference, cancellationToken);
        }
        else
        {
            preference.ReplaceValues(
                preferredStyles,
                preferredOccasions,
                favoriteColors,
                avoidedColors,
                _clock.UtcNow);
        }

        await _profiles.SaveChangesAsync(cancellationToken);

        return ProfileResult.Success(ToStylePreferencesResponse(preference));
    }

    private static ProfileResponse ToProfileResponse(UserProfile profile)
        => new(
            profile.UserId,
            profile.DisplayName,
            profile.AvatarObjectKey,
            profile.Gender,
            profile.HeightCm,
            profile.BodyShape);

    private static StylePreferencesResponse ToStylePreferencesResponse(UserStylePreference? preference)
        => new(
            preference?.GetValues(UserStylePreferenceValue.TypePreferredStyle) ?? [],
            preference?.GetValues(UserStylePreferenceValue.TypePreferredOccasion) ?? [],
            preference?.GetValues(UserStylePreferenceValue.TypeFavoriteColor) ?? [],
            preference?.GetValues(UserStylePreferenceValue.TypeAvoidedColor) ?? [],
            AllowedStyles.OrderBy(value => value).ToArray(),
            AllowedOccasions.OrderBy(value => value).ToArray());

    private static IReadOnlyCollection<string> Normalize(IReadOnlyCollection<string>? values)
        => values?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
            ?? [];

    private static string? EmptyToNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}