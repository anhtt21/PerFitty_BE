using Hnanut.PerFitty.Application.Abstractions;
using Hnanut.PerFitty.Application.Abstractions.Auth;
using Hnanut.PerFitty.Infrastructure.Auth;
using Hnanut.PerFitty.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hnanut.PerFitty.Application.Abstractions.Profile;

namespace Hnanut.PerFitty.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        });

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddSingleton<IPersistenceAvailabilityProbe>(
            new SqlServerPersistenceAvailabilityProbe(connectionString));
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IAuthTokenService, JwtAuthTokenService>();
        services.AddScoped<IProfileRepository, EfProfileRepository>();

        return services;
    }
}
