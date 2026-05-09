namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Permissão do catálogo atribuída a um perfil, com nível (full/own/limited/none).
/// </summary>
public class PerfilPermissaoCatalogo
{
    public int Id { get; set; }
    public int PerfilAcessoId { get; set; }
    public int PermissaoCatalogoId { get; set; }
    public string Nivel { get; set; } = "full";

    public PerfilAcesso PerfilAcesso { get; set; } = null!;
    public PermissaoCatalogo PermissaoCatalogo { get; set; } = null!;
}
