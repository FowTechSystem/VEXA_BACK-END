using CRMVEXA.Infrastructure.Data;

namespace CRMVEXA.Infrastructure.MultiTenancy;

public interface ITenantAppDbContextFactory
{
    AppDbContext CreateDbContext(int tenantId);
}
