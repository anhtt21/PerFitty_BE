using Hnanut.PerFitty.SharedKernel.Domain;

namespace Hnanut.PerFitty.Domain.Modules.Auth.Entities;

public sealed class UserProfile : Entity, IAuditableEntity
{
    private UserProfile()
    {
    }

    private UserProfile(Guid userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName.Trim();
    }

    public Guid UserId { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;

    public string? AvatarObjectKey { get; private set; }

    public string? Gender { get; private set; }

    public decimal? HeightCm { get; private set; }

    public string? BodyShape { get; private set; }

    public User User { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public static UserProfile Create(Guid userId, string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        return new UserProfile(userId, displayName);
    }

    public void UpdateBasicInfo(
        string displayName,
        string? avatarObjectKey,
        string? gender,
        decimal? heightCm,
        string? bodyShape,
        DateTimeOffset updatedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        DisplayName = displayName.Trim();
        AvatarObjectKey = avatarObjectKey;
        Gender = gender;
        HeightCm = heightCm;
        BodyShape = bodyShape;
        UpdatedAt = updatedAt;
    }
}