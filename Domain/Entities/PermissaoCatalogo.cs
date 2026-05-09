namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Catálogo global de permissões (sem EmpresaId). Chave estável para middleware e front (ex.: vendas.venda.criar).
/// </summary>
public class PermissaoCatalogo
{
    public int Id { get; set; }
    public string Modulo { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string Recurso { get; set; } = string.Empty;
    public string Chave { get; set; } = string.Empty;
    public string Rotulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public ICollection<PerfilPermissaoCatalogo> PerfilPermissoes { get; set; } = new List<PerfilPermissaoCatalogo>();
}
