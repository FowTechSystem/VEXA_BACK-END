using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CRMVEXA.Infrastructure.Data;

/// <summary>Permite <c>dotnet ef</c> no modelo do tenant sem subir a API.</summary>
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Defina ConnectionStrings:DefaultConnection para migrations do tenant.");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
            .Options;
        return new AppDbContext(options);
    }
}
