namespace Hnanut.PerFitty.Application.Features.Profile;

public sealed record ProfileResponse(
    Guid UserId,
    string DisplayName,
    string? AvatarObjectKey,
    string? Gender,
    decimal? HeightCm,
    string? BodyShape);

public sealed record UpdateProfileRequest(
    string DisplayName,
    string? AvatarObjectKey,
    string? Gender,
    decimal? HeightCm,
    string? BodyShape);

public sealed record StylePreferencesResponse(
    IReadOnlyCollection<string> PreferredStyles,
    IReadOnlyCollection<string> PreferredOccasions,
    IReadOnlyCollection<string> FavoriteColors,
    IReadOnlyCollection<string> AvoidedColors,
    IReadOnlyCollection<string> AvailableStyles,
    IReadOnlyCollection<string> AvailableOccasions);

public sealed record UpdateStylePreferencesRequest(
    IReadOnlyCollection<string>? PreferredStyles,
    IReadOnlyCollection<string>? PreferredOccasions,
    IReadOnlyCollection<string>? FavoriteColors,
    IReadOnlyCollection<string>? AvoidedColors);

public sealed record ProfileFailure(string Code, string Message);

public sealed class ProfileResult<T>
{
    internal ProfileResult(T? value, ProfileFailure? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }

    public ProfileFailure? Error { get; }

    public bool Succeeded => Error is null;
}

public static class ProfileResult
{
    public static ProfileResult<T> Success<T>(T value) => new(value, null);

    public static ProfileResult<T> Failure<T>(string code, string message)
        => new(default, new ProfileFailure(code, message));
}