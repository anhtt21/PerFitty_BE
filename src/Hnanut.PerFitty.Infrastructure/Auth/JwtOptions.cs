namespace Hnanut.PerFitty.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "PerFitty";

    public string Audience { get; init; } = "PerFitty.Mobile";

    public string Secret { get; init; } = string.Empty;

    public int AccessTokenMinutes { get; init; } = 15;

    public int RefreshTokenDays { get; init; } = 30;
}