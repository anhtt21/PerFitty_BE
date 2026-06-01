using Hnanut.PerFitty.SharedKernel.Domain;

namespace Hnanut.PerFitty.Domain.Modules.Auth.Entities;

public sealed class User : Entity, IAuditableEntity
{
    private readonly List<RefreshToken> _refreshTokens = [];

    private User()
    {
    }

    private User(string email, string passwordHash, string? displayName)
    {
        Email = email.Trim();
        NormalizedEmail = NormalizeEmail(email);
        PasswordHash = passwordHash;
        Profile = UserProfile.Create(Id, displayName ?? BuildDefaultDisplayName(email));
    }

    public string Email { get; private set; } = string.Empty;

    public string NormalizedEmail { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool EmailConfirmed { get; private set; }

    public bool IsDisabled { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    public UserProfile? Profile { get; private set; }

    public UserStylePreference? StylePreference { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public static User Create(string email, string passwordHash, string? displayName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new User(email, passwordHash, displayName);
    }

    public void ConfirmEmail(DateTimeOffset confirmedAt)
    {
        EmailConfirmed = true;
        UpdatedAt = confirmedAt;
    }

    public void MarkLogin(DateTimeOffset loginAt)
    {
        LastLoginAt = loginAt;
        UpdatedAt = loginAt;
    }

    public void Disable(DateTimeOffset disabledAt)
    {
        IsDisabled = true;
        UpdatedAt = disabledAt;
    }

    public void Enable(DateTimeOffset enabledAt)
    {
        IsDisabled = false;
        UpdatedAt = enabledAt;
    }

    public void ChangePasswordHash(string passwordHash, DateTimeOffset changedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        PasswordHash = passwordHash;
        UpdatedAt = changedAt;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }

    private static string BuildDefaultDisplayName(string email)
    {
        var atIndex = email.IndexOf('@', StringComparison.Ordinal);
        return atIndex > 0 ? email[..atIndex] : email;
    }
}