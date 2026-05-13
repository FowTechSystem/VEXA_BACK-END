namespace CRMVEXA.Application.Abstractions;

public interface IUserTenantDirectoryService
{
    Task<int?> GetTenantIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task UpsertAsync(int tenantId, string emailNormalized, int usuarioId, CancellationToken cancellationToken = default);
}
