using CRMVEXA.Application.Abstractions;
using CRMVEXA.Domain.Entities.Master;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.MultiTenancy;

public sealed class UserTenantDirectoryService : IUserTenantDirectoryService
{
    private readonly MasterDbContext _master;

    public UserTenantDirectoryService(MasterDbContext master)
    {
        _master = master;
    }

    public async Task<int?> GetTenantIdByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(email);
        if (string.IsNullOrEmpty(normalized))
            return null;

        return await _master.UserTenantDirectory.AsNoTracking()
            .Where(e => e.EmailNormalized == normalized)
            .Select(e => (int?)e.TenantId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpsertAsync(int tenantId, string emailNormalized, int usuarioId, CancellationToken cancellationToken = default)
    {
        var key = Normalize(emailNormalized);
        var existing = await _master.UserTenantDirectory
            .FirstOrDefaultAsync(e => e.EmailNormalized == key, cancellationToken);

        if (existing == null)
        {
            _master.UserTenantDirectory.Add(new UserTenantDirectoryEntry
            {
                TenantId = tenantId,
                EmailNormalized = key,
                UsuarioId = usuarioId
            });
        }
        else
        {
            existing.TenantId = tenantId;
            existing.UsuarioId = usuarioId;
        }

        await _master.SaveChangesAsync(cancellationToken);
    }

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
