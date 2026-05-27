using Hnanut.PerFitty.SharedKernel.Domain;

namespace Hnanut.PerFitty.Infrastructure.Persistence.Entities;

public sealed class AppSetting : IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Key { get; set; }

    public required string Value { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
