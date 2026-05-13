namespace CRMVEXA.Application.Abstractions;

public interface ITenantProvisioningService
{
    /// <summary>Cria registro no master, banco físico, migrations e linha inicial em Empresas.</summary>
    Task<int> ProvisionAsync(
        string nome,
        string? nomeFantasia,
        string? cnpj,
        string? email,
        string? telefone,
        bool ativo,
        CancellationToken cancellationToken = default);
}
