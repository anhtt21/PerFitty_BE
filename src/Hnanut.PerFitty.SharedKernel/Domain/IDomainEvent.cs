namespace Hnanut.PerFitty.SharedKernel.Domain;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
