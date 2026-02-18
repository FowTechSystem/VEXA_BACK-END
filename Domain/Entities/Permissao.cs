namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Tipo de permissão que pode ser atribuída a usuários (ex.: Visualizar relatórios, Gerenciar clientes).
/// </summary>
public class Permissao
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty; // ex: "VISUALIZAR_RELATORIOS"

    public ICollection<UsuarioPermissao> UsuarioPermissoes { get; set; } = new List<UsuarioPermissao>();

    private Permissao() { }

    public Permissao(string nome, string codigo)
    {
        Nome = nome;
        Codigo = codigo;
    }
}
