namespace CRMVEXA.Domain.Entities.Master;

/// <summary>Registro de tenant no banco catálogo (master).</summary>
public class TenantCatalog
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; }

    /// <summary>Nome do database PostgreSQL do tenant (ex.: crmvexa_tenant_3).</summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>Opcional: connection string completa protegida com Data Protection.</summary>
    public string? ConnectionStringProtected { get; set; }

    public DateTime? ProvisionadoEm { get; set; }
}
