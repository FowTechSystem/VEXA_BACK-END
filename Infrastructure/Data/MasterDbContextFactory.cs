using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CRMVEXA.Infrastructure.Data;

public sealed class MasterDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("MasterConnection")
            ?? "Host=localhost;Port=5433;Database=CRMVEXA_master;Username=postgres;Password=1234";

        var options = new DbContextOptionsBuilder<MasterDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new MasterDbContext(options);
    }
}
