using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hnanut.PerFitty.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("PERFITTY_CONNECTION_STRING")
            ?? "Server=localhost,1433;Database=PerFitty;User Id=sa;Password=Tanh0201@;TrustServerCertificate=True;Encrypt=False;Connect Timeout=5";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure();
            })
            .Options;

        return new AppDbContext(options);
    }
}
