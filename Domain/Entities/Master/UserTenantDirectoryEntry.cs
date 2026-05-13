namespace CRMVEXA.Domain.Entities.Master;

/// <summary>Mapeia e-mail (login) para o tenant que armazena o usuário.</summary>
public class UserTenantDirectoryEntry
{
    public int Id { get; set; }

    public int TenantId { get; set; }
    public TenantCatalog Tenant { get; set; } = null!;

    public string EmailNormalized { get; set; } = string.Empty;

    /// <summary>Id do usuário no banco do tenant.</summary>
    public int UsuarioId { get; set; }
}
