using Hnanut.PerFitty.Application.Abstractions;

namespace Hnanut.PerFitty.Application.Common;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
