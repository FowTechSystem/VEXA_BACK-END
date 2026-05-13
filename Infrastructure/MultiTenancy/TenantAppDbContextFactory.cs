using CRMVEXA.Application.Abstractions;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.MultiTenancy;

public sealed class TenantAppDbContextFactory : ITenantAppDbContextFactory
{
    private readonly ITenantConnectionResolver _resolver;

    public TenantAppDbContextFactory(ITenantConnectionResolver resolver)
    {
        _resolver = resolver;
    }

    public AppDbContext CreateDbContext(int tenantId)
    {
        var cs = _resolver.GetConnectionStringAsync(tenantId).GetAwaiter().GetResult();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs)
            .Options;
        return new AppDbContext(options);
    }
}
