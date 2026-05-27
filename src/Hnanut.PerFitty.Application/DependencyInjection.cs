using Hnanut.PerFitty.Application.Abstractions;
using Hnanut.PerFitty.Application.Common;
using Hnanut.PerFitty.Application.Features.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Hnanut.PerFitty.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}