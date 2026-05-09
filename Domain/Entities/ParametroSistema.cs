namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Parâmetro configurável por empresa (valor sempre texto; tipo indica parse no app).
/// </summary>
public class ParametroSistema
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Chave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string Tipo { get; set; } = "string";
    public string? Rotulo { get; set; }
    public string? Descricao { get; set; }
    public int? AtualizadoPorUsuarioId { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public Usuario? AtualizadoPorUsuario { get; set; }
}
