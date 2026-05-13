using CRMVEXA.Application.Abstractions;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRMVEXA.Infrastructure.MultiTenancy;

public sealed class TenantConnectionResolver : ITenantConnectionResolver
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private readonly IConnectionStringProtector? _protector;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

    public TenantConnectionResolver(
        IServiceScopeFactory scopeFactory,
        IMemoryCache cache,
        IConfiguration configuration,
        IConnectionStringProtector? protector = null)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
        _configuration = configuration;
        _protector = protector;
    }

    public async Task<string> GetConnectionStringAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"tenant-cs:{tenantId}";
        if (_cache.TryGetValue(cacheKey, out string? cached) && !string.IsNullOrEmpty(cached))
            return cached;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var master = scope.ServiceProvider.GetRequiredService<MasterDbContext>();

        var row = await master.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId && t.Ativo)
            .Select(t => new { t.ConnectionStringProtected, t.DatabaseName })
            .FirstOrDefaultAsync(cancellationToken);

        if (row == null)
            throw new InvalidOperationException($"Tenant {tenantId} não encontrado ou inativo.");

        string cs;
        if (!string.IsNullOrWhiteSpace(row.ConnectionStringProtected))
        {
            if (_protector == null)
                throw new InvalidOperationException("Connection string protegida configurada, mas IDataProtection não está registrado.");
            cs = _protector.Unprotect(row.ConnectionStringProtected);
        }
        else
        {
            var template = _configuration["ConnectionStrings:TenantConnectionTemplate"]
                ?? throw new InvalidOperationException("ConnectionStrings:TenantConnectionTemplate não configurado.");
            cs = template.Replace("{Database}", row.DatabaseName, StringComparison.OrdinalIgnoreCase);
        }

        _cache.Set(cacheKey, cs, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _ttl
        });

        return cs;
    }
}
