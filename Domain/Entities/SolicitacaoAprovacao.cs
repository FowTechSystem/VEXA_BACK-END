namespace CRMVEXA.Domain.Entities;

public class SolicitacaoAprovacao
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int SolicitadoPorUsuarioId { get; set; }
    public int? AprovadoPorUsuarioId { get; set; }
    public string PermissaoChave { get; set; } = string.Empty;
    public string? ContextoJson { get; set; }
    public string Estado { get; set; } = "pending";
    public string? Motivo { get; set; }
    public string? RespostaNota { get; set; }
    public DateTime? ExpiraEm { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? RespondidoEm { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public Usuario SolicitadoPorUsuario { get; set; } = null!;
    public Usuario? AprovadoPorUsuario { get; set; }
}
