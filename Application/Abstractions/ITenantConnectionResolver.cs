namespace CRMVEXA.Application.Abstractions;

public interface ITenantConnectionResolver
{
    Task<string> GetConnectionStringAsync(int tenantId, CancellationToken cancellationToken = default);
}
