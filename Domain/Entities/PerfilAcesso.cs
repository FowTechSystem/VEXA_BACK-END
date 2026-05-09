namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Perfil (role) por empresa — RBAC.
/// </summary>
public class PerfilAcesso
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    /// <summary>Nome interno único por empresa (ex.: admin, gerente).</summary>
    public string Nome { get; set; } = string.Empty;
    public string Rotulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool PadraoNovoUsuario { get; set; }
    public bool Ativo { get; set; } = true;

    public Empresa Empresa { get; set; } = null!;
    public ICollection<PerfilPermissaoCatalogo> PerfilPermissoes { get; set; } = new List<PerfilPermissaoCatalogo>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
