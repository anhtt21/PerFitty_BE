namespace Hnanut.PerFitty.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
