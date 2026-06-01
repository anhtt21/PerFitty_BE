using Hnanut.PerFitty.Application.Abstractions;
using Hnanut.PerFitty.Application.Common;
using Hnanut.PerFitty.Application.Features.Auth;
using Microsoft.Extensions.DependencyInjection;
using Hnanut.PerFitty.Application.Features.Profile;

namespace Hnanut.PerFitty.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfileService, ProfileService>();

        return services;
    }
}