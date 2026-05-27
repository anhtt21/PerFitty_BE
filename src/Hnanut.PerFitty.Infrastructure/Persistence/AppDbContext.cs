using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Hnanut.PerFitty.Infrastructure.Persistence.Entities;
using Hnanut.PerFitty.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Hnanut.PerFitty.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.UseSnakeCaseNames();

        base.OnModelCreating(modelBuilder);
    }
}