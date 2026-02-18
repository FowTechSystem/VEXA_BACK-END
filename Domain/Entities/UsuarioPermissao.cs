namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Relacionamento N:N entre Usuario e Permissao (quais permissões o usuário possui).
/// </summary>
public class UsuarioPermissao
{
    public int UsuarioId { get; set; }
    public int PermissaoId { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Permissao Permissao { get; set; } = null!;
}
