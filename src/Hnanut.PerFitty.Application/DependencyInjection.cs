using Hnanut.PerFitty.Application.Abstractions;
using Hnanut.PerFitty.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Hnanut.PerFitty.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
