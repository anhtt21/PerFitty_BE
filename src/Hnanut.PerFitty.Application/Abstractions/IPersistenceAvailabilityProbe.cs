namespace Hnanut.PerFitty.Application.Abstractions;

public interface IPersistenceAvailabilityProbe
{
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}
