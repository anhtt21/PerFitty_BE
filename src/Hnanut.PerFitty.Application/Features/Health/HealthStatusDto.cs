namespace Hnanut.PerFitty.Application.Features.Health;

public sealed record HealthStatusDto(string Service, string Status, DateTimeOffset UtcNow);
