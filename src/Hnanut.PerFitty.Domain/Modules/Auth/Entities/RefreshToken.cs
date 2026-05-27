using Hnanut.PerFitty.SharedKernel.Domain;

namespace Hnanut.PerFitty.Domain.Modules.Auth.Entities;

public sealed class RefreshToken : Entity, IAuditableEntity
{
    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid userId,
        string tokenHash,
        Guid tokenFamilyId,
        DateTimeOffset expiresAt,
        string? deviceId,
        string? deviceName,
        string? ipAddress,
        string? userAgent)
    {
        UserId = userId;
        TokenHash = tokenHash;
        TokenFamilyId = tokenFamilyId;
        ExpiresAt = expiresAt;
        DeviceId = deviceId;
        DeviceName = deviceName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public Guid TokenFamilyId { get; private set; }

    public string? DeviceId { get; private set; }

    public string? DeviceName { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public string? ReplacedByTokenHash { get; private set; }

    public User User { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsExpired(DateTimeOffset now)
    {
        return now >= ExpiresAt;
    }

    public bool IsActive(DateTimeOffset now)
    {
        return !IsRevoked && !IsExpired(now);
    }

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        string? deviceId = null,
        string? deviceName = null,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? tokenFamilyId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        return new RefreshToken(
            userId,
            tokenHash,
            tokenFamilyId ?? Guid.NewGuid(),
            expiresAt,
            deviceId,
            deviceName,
            ipAddress,
            userAgent);
    }

    public void Revoke(DateTimeOffset revokedAt, string? replacedByTokenHash = null)
    {
        RevokedAt = revokedAt;
        ReplacedByTokenHash = replacedByTokenHash;
        UpdatedAt = revokedAt;
    }
}