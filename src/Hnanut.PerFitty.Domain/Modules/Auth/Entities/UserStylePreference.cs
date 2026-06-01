using Hnanut.PerFitty.SharedKernel.Domain;

namespace Hnanut.PerFitty.Domain.Modules.Auth.Entities;

public sealed class UserStylePreference : Entity, IAuditableEntity
{
    private readonly List<UserStylePreferenceValue> _values = [];

    private UserStylePreference()
    {
    }

    private UserStylePreference(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;

    public IReadOnlyCollection<UserStylePreferenceValue> Values => _values.AsReadOnly();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public static UserStylePreference Create(
        Guid userId,
        IEnumerable<string> preferredStyles,
        IEnumerable<string> preferredOccasions,
        IEnumerable<string> favoriteColors,
        IEnumerable<string> avoidedColors,
        DateTimeOffset createdAt)
    {
        var preference = new UserStylePreference(userId)
        {
            CreatedAt = createdAt
        };

        preference.ReplaceValues(
            preferredStyles,
            preferredOccasions,
            favoriteColors,
            avoidedColors,
            createdAt);

        return preference;
    }

    public void ReplaceValues(
        IEnumerable<string> preferredStyles,
        IEnumerable<string> preferredOccasions,
        IEnumerable<string> favoriteColors,
        IEnumerable<string> avoidedColors,
        DateTimeOffset updatedAt)
    {
        _values.Clear();

        AddValues(UserStylePreferenceValue.TypePreferredStyle, preferredStyles);
        AddValues(UserStylePreferenceValue.TypePreferredOccasion, preferredOccasions);
        AddValues(UserStylePreferenceValue.TypeFavoriteColor, favoriteColors);
        AddValues(UserStylePreferenceValue.TypeAvoidedColor, avoidedColors);

        UpdatedAt = updatedAt;
    }

    public IReadOnlyCollection<string> GetValues(string preferenceType)
    {
        return _values
            .Where(value => value.PreferenceType == preferenceType)
            .Select(value => value.Value)
            .ToArray();
    }

    private void AddValues(string preferenceType, IEnumerable<string> values)
    {
        foreach (var value in Normalize(values))
        {
            _values.Add(UserStylePreferenceValue.Create(Id, preferenceType, value));
        }
    }

    private static IEnumerable<string> Normalize(IEnumerable<string> values)
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class UserStylePreferenceValue : Entity
{
    public const string TypePreferredStyle = "preferred_style";
    public const string TypePreferredOccasion = "preferred_occasion";
    public const string TypeFavoriteColor = "favorite_color";
    public const string TypeAvoidedColor = "avoided_color";

    private UserStylePreferenceValue()
    {
    }

    private UserStylePreferenceValue(
        Guid userStylePreferenceId,
        string preferenceType,
        string value)
    {
        UserStylePreferenceId = userStylePreferenceId;
        PreferenceType = preferenceType;
        Value = value;
    }

    public Guid UserStylePreferenceId { get; private set; }

    public string PreferenceType { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public UserStylePreference StylePreference { get; private set; } = null!;

    public static UserStylePreferenceValue Create(
        Guid userStylePreferenceId,
        string preferenceType,
        string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(preferenceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return new UserStylePreferenceValue(
            userStylePreferenceId,
            preferenceType.Trim(),
            value.Trim().ToLowerInvariant());
    }
}